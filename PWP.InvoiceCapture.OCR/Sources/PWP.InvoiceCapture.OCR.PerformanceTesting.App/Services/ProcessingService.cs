using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Definitions;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services
{
    internal class ProcessingService : IProcessingService
    {
        public ProcessingService(
            IDocumentAggregationClient documentAggregationClient,
            IInvoiceManagementClient invoiceManagementClient,
            IOptions<Settings> settingsAccessor,
            IDataAnnotationComparisonService dataAnnotationComparisonService,
            IReportingService reportingService)
        {
            this.invoiceManagementClient = invoiceManagementClient;
            this.documentAggregationClient = documentAggregationClient;
            this.dataAnnotationComparisonService = dataAnnotationComparisonService;
            this.reportingService = reportingService;
            settings = settingsAccessor.Value;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var folderPaths = Directory.GetDirectories(settings.FolderPath).ToList();

            producerConsumer.Produce(folderPaths);
            await producerConsumer.ConsumeAsync(ProcessFolderAsync, settings.ThreadsCount, cancellationToken);
           
            var results = new RecognitionTestResult()
            {
                InvoiceGroups = invoiceGroups.ToList()
            };
           
            await SaveResultAsync(results);
        }

        private async Task ProcessFolderAsync(string folderPath, CancellationToken cancellationToken)
        {
            var configurationFilePath = Path.Combine(folderPath, "configuration.json");
            var groupConfiguration = DeserializeFileToJson<InvoiceGroup>(configurationFilePath);
            var invoiceProcessingResults = new List<InvoiceRecognitionResult>();
            var uploadAttempt = 1;

            foreach (var invoice in groupConfiguration.Invoices)
            {
                var invoiceProcessingResult = await ProcessFileAsync(folderPath, invoice, cancellationToken);
                invoiceProcessingResults.Add(invoiceProcessingResult);

                await DelayInvoiceProcessingAsync(uploadAttempt);
                uploadAttempt++;
            }

            var groupResult = new InvoiceGroupRecognitionResult
            {
                Folder = Path.GetFileName(folderPath),
                Results = invoiceProcessingResults, 
            };

            invoiceGroups.Add(groupResult);
        }

        private async Task DelayInvoiceProcessingAsync(int uploadAttempt) 
        {
            if (uploadAttempt == CommonDefinitions.UploadAttemptToCheckRecognition - 1)
            {
                await Task.Delay(modelCreationDelayMilliseconds);
            }

            await Task.Delay(settings.InvoiceUploadingIntervalMilliseconds);
        }

        private async Task<InvoiceRecognitionResult> ProcessFileAsync(string folderPath, Models.Invoice invoiceDefinition, CancellationToken cancellationToken) 
        {
            var invoicePath = Path.Combine(folderPath, invoiceDefinition.FileName);
            var documentId = string.Empty;
            
            var stopwatch = new Stopwatch();
            using (var fileStream = File.OpenRead(invoicePath))
            {
                stopwatch.Start();
                var response = await documentAggregationClient.UploadFileAsync(fileStream, invoiceDefinition.FileName, cancellationToken);
                documentId = response.Data.DocumentId;
            }

            var invoice = await GetInvoiceInPendingReviewStatusAsync(documentId, cancellationToken);
            var timeElapsed = stopwatch.Elapsed;

            if (invoice == null || invoice.Status != InvoiceStatus.PendingReview)
            {
                return CreateErrorInvoiceRecognitionResult(invoiceDefinition.FileName, timeElapsed);
            }

            var processingResult = await invoiceManagementClient.GetLatestProcessingResultByInvoiceIdAsync(invoice.Id, cancellationToken);
            var expectedAnnotationPath = Path.Combine(folderPath, $"{Path.GetFileNameWithoutExtension(invoiceDefinition.FileName)}.json");
            var expectedAnnotation = DeserializeFileToJson<DataAnnotation>(expectedAnnotationPath);

            await CompleteInvoiceAsync(processingResult, expectedAnnotation, cancellationToken);

            var comparisonResult = dataAnnotationComparisonService.GetComparisonResult(expectedAnnotation, processingResult.DataAnnotation);

            comparisonResult.FileName = invoiceDefinition.FileName;
            comparisonResult.OcrTemplateId = processingResult.TemplateId;
            comparisonResult.TimeElapsed = timeElapsed;

            return comparisonResult;
        }

        private async Task CompleteInvoiceAsync(InvoiceProcessingResult processingResult, DataAnnotation expectedAnnotation, CancellationToken cancellationToken) 
        {
            try
            {
                await invoiceManagementClient.CompleteAsync(processingResult.Id, expectedAnnotation, cancellationToken);
            }
            catch (HttpRequestException) 
            {
                // There is a chance that invoice has been completed, but we lost the connection. So we'll get BadRequest on retry since invoice is already completed.
                // For this specific case we check whether invoice status is completed or not
                var invoice = await invoiceManagementClient.GetInvoiceByIdAsync(processingResult.InvoiceId, cancellationToken);

                if (invoice.Status != InvoiceStatus.Completed)
                {
                    throw;
                }
            }
        }

        private async Task<InvoiceManagement.Business.Contract.Models.Invoice> GetInvoiceInPendingReviewStatusAsync(string documentId, CancellationToken cancellationToken)
        {
            var elapsedMilliseconds = 0;

            while (elapsedMilliseconds < invoicePollingTimeout.TotalMilliseconds)
            {
                elapsedMilliseconds += settings.InvoicePollingIntervalMilliseconds;
                await Task.Delay(settings.InvoicePollingIntervalMilliseconds);

                var invoice = await invoiceManagementClient.GetInvoiceByDocumentIdAsync(documentId, cancellationToken);

                // Invoice is created asynchronously so it's possible invoice is not found by documentId
                if (invoice == null)
                {
                    continue;
                }

                switch (invoice.Status)
                {
                    case InvoiceStatus.NotStarted:
                    case InvoiceStatus.Queued:
                    case InvoiceStatus.InProgress:
                        continue;
                }

                return invoice;
            }

            return null;
        }

        private TEntity DeserializeFileToJson<TEntity>(string filePath)
        {
            var json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<TEntity>(json);
        }

        private async Task SaveResultAsync(RecognitionTestResult recognitionResults)
        {
            var reportBytes = reportingService.Create(recognitionResults);
            var filename = $"OCR Performance Results - Tenant{settings.TenantId}-{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")}";
            var filePath = Path.Combine(settings.FolderPath, filename);

            // Save JSON results
            File.WriteAllText($"{filePath}.json", JsonConvert.SerializeObject(recognitionResults, Formatting.Indented));

            // Save reporting service results (.xlsx)
            using (var reportStream = new MemoryStream(reportBytes))
            using (var fileStream = new FileStream($"{filePath}{reportingService.Extension}", FileMode.CreateNew))
            {
                await reportStream.CopyToAsync(fileStream);
            }
        }

        private InvoiceRecognitionResult CreateErrorInvoiceRecognitionResult(string fileName, TimeSpan timeElapsed) => new InvoiceRecognitionResult
        {
            FileName = fileName,
            OcrTemplateId = "Error",
            TimeElapsed = timeElapsed,
            TotalFieldsCount = 0,
            CorrectlyAssignedFieldsCount = -1
        };

        private readonly IDocumentAggregationClient documentAggregationClient;
        private readonly IInvoiceManagementClient invoiceManagementClient;
        private readonly IDataAnnotationComparisonService dataAnnotationComparisonService;
        private readonly IReportingService reportingService;
        private readonly Settings settings;
        private readonly ConcurrentBag<InvoiceGroupRecognitionResult> invoiceGroups = new ConcurrentBag<InvoiceGroupRecognitionResult>();
        private readonly ProducerConsumer<string> producerConsumer = new ProducerConsumer<string>();
        private readonly TimeSpan invoicePollingTimeout = TimeSpan.FromMinutes(10);
        private readonly int modelCreationDelayMilliseconds = 120000;
    } 
}
