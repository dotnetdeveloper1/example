using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories;
using PWP.InvoiceCapture.OCR.Core.Extensions;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.Business.Services
{
    internal class TemplateManagementService : ITemplateManagementService
    {
        public TemplateManagementService(
            IDocumentApiClient documentApiClient, 
            ISerializationService serializationService, 
            IInvoiceTemplateRepository invoiceTemplateRepository,
            IFormRecognizerTrainingDocumentFactory formRecognizerTrainingDocumentFactory, 
            ITrainingBlobRepository trainingBlobRepository, 
            IFormRecognizerClient formRecognizerClient,
            ITelemetryClient telemetryClient, 
            IFileNameProvider fileNameProvider)
        {
            this.documentApiClient = documentApiClient;
            this.serializationService = serializationService;
            this.invoiceTemplateRepository = invoiceTemplateRepository;
            this.formRecognizerTrainingDocumentFactory = formRecognizerTrainingDocumentFactory;
            this.trainingBlobRepository = trainingBlobRepository;
            this.formRecognizerClient = formRecognizerClient;
            this.telemetryClient = telemetryClient;
            this.fileNameProvider = fileNameProvider;
        }

        public async Task<int> GetTemplateTrainingsCountAsync(int templateId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(templateId, nameof(templateId));

            var template = await invoiceTemplateRepository.GetByIdAsync(templateId, cancellationToken);

            if (template == null)
            {
                return 0;
            }

            return template.TrainingFileCount;
        }

        public async Task<InvoiceTemplate> GetTemplateAsync(int templateId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(templateId, nameof(templateId));

            return await invoiceTemplateRepository.GetByIdAsync(templateId, cancellationToken);
        }

        public async Task ProcessUserValidationDataAsync(int invoiceId, string fileId, string annotationFileId, int templateId, string tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));
            Guard.IsNotNullOrWhiteSpace(annotationFileId, nameof(annotationFileId));
            Guard.IsNotZeroOrNegative(templateId, nameof(templateId));
            Guard.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));
            
            var telemetry = TelemetryData.Create(invoiceId, tenantId, fileId);
            telemetryClient.TrackTrace("Template training service started.", telemetry);

            var dataAnnotation = await GetDataAnnotationAsync(annotationFileId, cancellationToken);

            if (!HasAnyAnnotationCreatedByUser(dataAnnotation))
            {
                return;
            }

            var template = await CreateBlobContainerForInvoiceTemplateIfNotExistsAsync(templateId, cancellationToken);
            var trainingFileName = await UploadTrainingFilesAsync(template, fileId, dataAnnotation, cancellationToken);
            var trainingFilesCount = await GetTrainingFilesCountAsync(template.TrainingBlobUri, cancellationToken);
            var shouldModelBeCreated = await CheckModelShouldBeCreatedAndUpdateTemplateAsync(template, trainingFilesCount, cancellationToken);

            if (!shouldModelBeCreated)
            {
                return;
            }

            // generate sas token for form recognizer to access the blob
            var sasUri = trainingBlobRepository.GetSasUri(template.TrainingBlobUri);

            telemetryClient.TrackTrace("Trying to traing model..", telemetry);

            // train new model and update the template blob container
            var modelResponse = await formRecognizerClient.TrainModelAsync(sasUri, cancellationToken);

            TryLogTrainingResponse(modelResponse, telemetry);

            // wait until model is ready
            var modelReadinessResponse = await formRecognizerClient.AwaitModelReadinessAsync(modelResponse.ModelInfo.ModelId, formRecognizerClient.FormRecognizerId, cancellationToken);

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive row lock on Template to be sure other transactions cannot read or modify it till this transaction ends.
                await invoiceTemplateRepository.LockAsync(templateId, cancellationToken);

                template = await invoiceTemplateRepository.GetByIdAsync(templateId, cancellationToken);
                    
                if (modelReadinessResponse.IsModelReady)
                {
                    await DeleteOldModelAndUpdateInvoiceTemplateAsync(template, trainingFilesCount, modelResponse.ModelInfo.ModelId, formRecognizerClient.FormRecognizerId, telemetry, cancellationToken);
                }
                else
                {
                    await DeleteBrokenFilesAsync(template, modelResponse, fileId, trainingFileName, telemetry, cancellationToken);
                }

                transaction.Complete();
            }

            telemetryClient.TrackTrace("Template training service finished.", telemetry);
        }

        private async Task DeleteBrokenFilesAsync(InvoiceTemplate template, TrainModelResponse modelResponse, string fileId, string trainingFileName, TelemetryData telemetry, CancellationToken cancellationToken) 
        {
            telemetryClient.TrackTrace("Model is not ready.", telemetry);

            if (modelResponse.TrainResult == null || modelResponse.TrainResult.Errors == null)
            {
                return;
            }

            if (modelResponse.TrainResult.Errors.Count > 0)
            {
                // Some training file or files are breaking the training process
                // If a new file has been added to a repo with a functioning model we remove that file and keep the old model
                if (!string.IsNullOrEmpty(template.FormRecognizerModelId))
                {
                    telemetryClient.TrackTrace("Current model exists. Deleting new training files which could cause the error.", telemetry);

                    var ocrFileName = fileNameProvider.CreateTemporaryFileName(fileId);

                    await trainingBlobRepository.DeleteAsync(template.TrainingBlobUri, fileId, cancellationToken);
                    await trainingBlobRepository.DeleteAsync(template.TrainingBlobUri, ocrFileName, cancellationToken);
                    await trainingBlobRepository.DeleteAsync(template.TrainingBlobUri, trainingFileName, cancellationToken);
                }
                // If this is a new training repo (this is the 5th file), we cannot know which file is creating the problem so we have to delete all files
                else
                {
                    telemetryClient.TrackTrace("No model exists. Deleting all training files.", telemetry);

                    // delete contents of training repo, set training file count to 0
                    await trainingBlobRepository.DeleteAllBlobsAsync(template.TrainingBlobUri, cancellationToken);
                    template.TrainingFileCount = 0;

                    await invoiceTemplateRepository.UpdateAsync(template, cancellationToken);
                }
            }
        }

        private async Task DeleteOldModelAndUpdateInvoiceTemplateAsync(InvoiceTemplate template, int trainingFilesCount, string modelId, int formRecognizerId, TelemetryData telemetry, CancellationToken cancellationToken)
        {
            telemetryClient.TrackTrace("Model is ready.", telemetry);

            if (trainingFilesCount <= template.TrainingFileCount)
            {
                telemetryClient.TrackTrace($"New model is not required, it was trained in parrallel. Trying to delete model ID = {modelId}", telemetry);
                await formRecognizerClient.DeleteModelAsync(modelId, formRecognizerId, cancellationToken);
                telemetryClient.TrackTrace($"Model ID = {modelId} has been deleted.", telemetry);

                return;
            }

            // Delete old model
            if (!string.IsNullOrWhiteSpace(template.FormRecognizerModelId) && template.FormRecognizerId.HasValue)
            {
                telemetryClient.TrackTrace($"Trying to delete old model ID = {template.FormRecognizerModelId}", telemetry);
                await formRecognizerClient.DeleteModelAsync(template.FormRecognizerModelId, template.FormRecognizerId.Value, cancellationToken);
                telemetryClient.TrackTrace($"Old model ID = {template.FormRecognizerModelId} has been deleted.", telemetry);
            }

            template.FormRecognizerModelId = modelId;
            template.FormRecognizerId = formRecognizerId;
            template.TrainingFileCount = trainingFilesCount;

            await invoiceTemplateRepository.UpdateAsync(template, cancellationToken);
        }

        private async Task<bool> CheckModelShouldBeCreatedAndUpdateTemplateAsync(InvoiceTemplate template, int trainingFilesCount, CancellationToken cancellationToken)
        {
            if (trainingFilesCount >= modelCreationFileCount)
            {
                return true;
            }

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive row lock on Template to be sure other transactions cannot read or modify it till this transaction ends.
                await invoiceTemplateRepository.LockAsync(template.Id, cancellationToken);

                template = await invoiceTemplateRepository.GetByIdAsync(template.Id, cancellationToken);

                if (trainingFilesCount > template.TrainingFileCount)
                {
                    template.TrainingFileCount = trainingFilesCount;

                    await invoiceTemplateRepository.UpdateAsync(template, cancellationToken);
                }

                transaction.Complete();
            }

            return false;
        }

        private async Task<string> UploadTrainingFilesAsync(InvoiceTemplate template, string fileId, DataAnnotation dataAnnotation, CancellationToken cancellationToken)
        {
            // Upload pdf file to training blob
            using (var documentStream = await documentApiClient.GetDocumentStreamAsync(fileId, cancellationToken))
            {
                await trainingBlobRepository.UploadAsync(template.TrainingBlobUri, fileId, documentStream, cancellationToken);
            }

            // Upload form recognizer response from temp container to the training repository
            await trainingBlobRepository.MoveTemporaryFileAsync(fileId, template.TrainingBlobUri, cancellationToken);

            // Create the training file and upload it to the blob 
            FormReconizerTrainingDocument trainingFile;
            trainingFile = formRecognizerTrainingDocumentFactory.Create(fileId, dataAnnotation);
            var camelCaseSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var trainingContent = JsonConvert.SerializeObject(trainingFile, camelCaseSettings);
            var trainingFileSuffix = fileNameProvider.GetTrainingFileSuffix();
            var trainingFileName = $"{fileId}{trainingFileSuffix}";

            using (var documentStream = trainingContent.ToStream())
            {
                await trainingBlobRepository.UploadAsync(template.TrainingBlobUri, trainingFileName, documentStream, cancellationToken);
            }

            return trainingFileName;
        }

        private async Task<InvoiceTemplate> CreateBlobContainerForInvoiceTemplateIfNotExistsAsync(int templateId, CancellationToken cancellationToken)
        {
            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive row lock on Template to be sure other transactions cannot read or modify it till this transaction ends.
                await invoiceTemplateRepository.LockAsync(templateId, cancellationToken);

                var template = await invoiceTemplateRepository.GetByIdAsync(templateId, cancellationToken);

                if (string.IsNullOrEmpty(template.TrainingBlobUri))
                {
                    var blobName = fileNameProvider.GetBlobContainerName(template.Id.ToString());
                    template.TrainingBlobUri = blobName;

                    await trainingBlobRepository.CreateBlobContainerAsync(blobName, cancellationToken);
                    await invoiceTemplateRepository.UpdateAsync(template, cancellationToken);
                }

                transaction.Complete();

                return template;
            }
        }

        private bool HasAnyAnnotationCreatedByUser(DataAnnotation dataAnnotation) 
        {
            return
                HasUserCreatedAnnotation(dataAnnotation.InvoiceAnnotations) ||
                dataAnnotation.InvoiceLineAnnotations.Any(lineAnnotation => HasUserCreatedAnnotation(lineAnnotation.LineItemAnnotations));
        }

        private async Task<DataAnnotation> GetDataAnnotationAsync(string annotationFileId, CancellationToken cancellationToken)
        {
            using (var annotationFileStream = await documentApiClient.GetDocumentStreamAsync(annotationFileId, cancellationToken))
            {
                var reader = new StreamReader(annotationFileStream);

                return serializationService.Deserialize<DataAnnotation>(reader.ReadToEnd());
            }
        }

        private void TryLogTrainingResponse(TrainModelResponse modelResponse, TelemetryData telemetry) 
        {
            try
            {
                var content = JsonConvert.SerializeObject(modelResponse);
                telemetryClient.TrackTrace(content, telemetry);
            }
            catch(Exception exception) 
            {
                telemetryClient.TrackTrace($"Exception was thrown while trying to log TrainModelResponse object: {exception.Message}", telemetry);
            }
        }

        private async Task<int> GetTrainingFilesCountAsync(string containerName, CancellationToken cancellationToken) 
        {
            var documentsCount = await trainingBlobRepository.GetBlobsCountAsync(containerName, cancellationToken);
            var trainingFilesCount = documentsCount / numberOfFilesPerDocument;

            return trainingFilesCount;
        }

        private bool HasUserCreatedAnnotation(List<Annotation> annotations) => annotations.Any(annotation => annotation.UserCreated);

        private readonly int modelCreationFileCount = 5;
        private readonly int numberOfFilesPerDocument = 3;
        private readonly IDocumentApiClient documentApiClient;
        private readonly ISerializationService serializationService;
        private readonly IInvoiceTemplateRepository invoiceTemplateRepository;
        private readonly IFormRecognizerTrainingDocumentFactory formRecognizerTrainingDocumentFactory;
        private readonly ITrainingBlobRepository trainingBlobRepository;
        private readonly IFormRecognizerClient formRecognizerClient;
        private readonly ITelemetryClient telemetryClient;
        private readonly IFileNameProvider fileNameProvider;
    }
}
