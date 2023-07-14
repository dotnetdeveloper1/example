using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services
{
    internal class CollectorService : ICollectorService
    {
        public CollectorService(IInvoiceManagementClient invoiceManagementClient, IInvoiceProcessingResultRepository repository, IOptions<Settings> settingsAccessor) 
        {
            this.invoiceManagementClient = invoiceManagementClient;
            this.repository = repository;
            settings = settingsAccessor.Value;
        }

        public async Task CollectAsync(CancellationToken cancellationToken)
        {
            var completedInvoicesResults = await repository.GetCompletedListAsync(cancellationToken);
            var groupedResults = completedInvoicesResults
                .GroupBy(result => result.TemplateId)
                .Select(group => KeyValuePair.Create(group.Key, group.ToList()))
                .ToList();

            producerConsumer.Produce(groupedResults);

            await producerConsumer.ConsumeAsync(CollectGroupAsync, settings.ThreadsCount, cancellationToken);
        }

        private async Task CollectGroupAsync(KeyValuePair<string, List<InvoiceProcessingResult>> invoiceGroup, CancellationToken cancellationToken) 
        {
            var path = Path.Combine(settings.FolderPath, invoiceGroup.Key);

            CreateDirectory(path);
            CreateConfigurationFile(path, invoiceGroup.Value);

            // Assuming FileName is unique per group and there can not be different invoices with the same name
            var invoiceIds = invoiceGroup.Value
                .GroupBy(result => result.Invoice.FileName.ToUpper())
                .Select(group => group.First().InvoiceId)
                .ToList();

            foreach (var invoiceId in invoiceIds)
            {
                await CollectInvoiceAsync(path, invoiceId, cancellationToken);
            }
        }

        private void CreateDirectory(string path) 
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void CreateConfigurationFile(string path, List<InvoiceProcessingResult> invoiceProcessingResults) 
        {
            var invoices = invoiceProcessingResults
                .OrderBy(result => result.ModifiedDate)
                .Select(result => 
                    new Models.Invoice
                    {
                        FileName = result.Invoice.FileName
                    })
                .ToList();

            var invoiceGroup = new InvoiceGroup { Invoices = invoices };
            var filePath = Path.Combine(path, "configuration.json");

            SerializeJsonToFile(filePath, invoiceGroup);
        }

        private void SerializeJsonToFile(string filePath, object model) 
        {
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);

            File.WriteAllText(filePath, json);
        }

        private async Task CollectInvoiceAsync(string path, int invoiceId, CancellationToken cancellationToken) 
        {
            var resultWithDataAnnotation = await invoiceManagementClient.GetLatestProcessingResultByInvoiceIdAsync(invoiceId, cancellationToken);

            CreateDataAnnotationFile(path, resultWithDataAnnotation);
            await CreateInvoiceFileAsync(path, resultWithDataAnnotation, cancellationToken);
        }

        private void CreateDataAnnotationFile(string path, InvoiceProcessingResult invoiceProcessingResult) 
        {
            var filePath = Path.Combine(path, $"{invoiceProcessingResult.Invoice.Name}.json");
            
            SerializeJsonToFile(filePath, invoiceProcessingResult.DataAnnotation);
        }

        private async Task CreateInvoiceFileAsync(string path, InvoiceProcessingResult invoiceProcessingResult, CancellationToken cancellationToken) 
        {
            var documentLink = await invoiceManagementClient.GetDocumentFileLinkAsync(invoiceProcessingResult.InvoiceId, cancellationToken);
            var documentResponse = await httpClient.GetAsync(documentLink);

            documentResponse.EnsureSuccessStatusCode();

            var filePath = Path.Combine(path, invoiceProcessingResult.Invoice.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
            {
                await documentResponse.Content.CopyToAsync(fileStream);
            }
        }

        private readonly IInvoiceManagementClient invoiceManagementClient;
        private readonly IInvoiceProcessingResultRepository repository;
        private readonly Settings settings;
        private readonly ProducerConsumer<KeyValuePair<string, List<InvoiceProcessingResult>>> producerConsumer = new ProducerConsumer<KeyValuePair<string, List<InvoiceProcessingResult>>>();
        private readonly static HttpClient httpClient = new HttpClient();
    }
}
