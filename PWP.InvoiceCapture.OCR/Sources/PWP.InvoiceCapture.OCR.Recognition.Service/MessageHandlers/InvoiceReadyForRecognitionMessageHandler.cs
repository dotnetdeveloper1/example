using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.Extensions;
using System.Threading;
using System.Threading.Tasks;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Mappers;

namespace PWP.InvoiceCapture.OCR.Recognition.Service.MessageHandlers
{
    public class InvoiceReadyForRecognitionMessageHandler : MessageHandlerBase<InvoiceReadyForRecognitionMessage>
    {
        public InvoiceReadyForRecognitionMessageHandler(IFieldMapper fieldMapper, IServiceBusPublisher publisher, ITelemetryClient telemetryClient, IRecognitionEngine recognitionEngine,
            IDocumentApiClient documentApiClient, ISerializationService serializationService, IApplicationContext applicationContext) : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(publisher, nameof(publisher));
            Guard.IsNotNull(recognitionEngine, nameof(recognitionEngine));
            Guard.IsNotNull(documentApiClient, nameof(documentApiClient));
            Guard.IsNotNull(applicationContext, nameof(applicationContext));
            Guard.IsNotNull(fieldMapper, nameof(fieldMapper));

            this.publisher = publisher;
            this.recognitionEngine = recognitionEngine;
            this.documentApiClient = documentApiClient;
            this.serializationService = serializationService;
            this.fieldMapper = fieldMapper;
        }

        protected override async Task HandleMessageAsync(InvoiceReadyForRecognitionMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotNull(message.Fields, nameof(message.Fields));
            Guard.IsNotNullOrWhiteSpace(message.FileId, nameof(message.FileId));
            Guard.IsNotZeroOrNegative(message.InvoiceId, nameof(message.InvoiceId));

            await PublishInvoiceProcessingStartedMessageAsync(message.InvoiceId, cancellationToken);

            var fieldTargetFields = fieldMapper.ToFieldTargetFieldList(message.Fields);

            var recognitionOperationResult = await recognitionEngine.ProcessDocumentAsync(fieldTargetFields, message.InvoiceId, message.FileId, message.Pages[0].ImageFileId, applicationContext.TenantId, cancellationToken);

            if (!recognitionOperationResult.IsSuccessful)
            {
                await PublishInvoiceProcessingErrorMessageAsync(message.InvoiceId, recognitionOperationResult.Message, cancellationToken);

                return;
            }

            var recognitionResult = recognitionOperationResult.Data;
            var fileId = await UploadDataAnnotationAsync(recognitionResult, cancellationToken);

            await PublishInvoiceRecognitionCompletedMessageAsync(message, recognitionResult, fileId, cancellationToken);
        }

        private async Task<string> UploadDataAnnotationAsync(RecognitionEngineResponse recognitionResult, CancellationToken cancellationToken) 
        {
            var serializedAnnotations = serializationService.Serialize(recognitionResult.DataAnnotation);
            ApiResponse<UploadDocumentResponse> uploadResult;

            using (var annotationsStream = serializedAnnotations.ToStream())
            {
                uploadResult = await documentApiClient.UploadDocumentAsync(annotationsStream, annotationsFileName, cancellationToken);
            }

            return uploadResult.Data.FileId;
        }

        private async Task PublishInvoiceProcessingStartedMessageAsync(int invoiceId, CancellationToken cancellationToken) 
        {
            var processingStartedMessage = new InvoiceProcessingStartedMessage
            {
                InvoiceId = invoiceId,
                TenantId = applicationContext.TenantId
            };

            await publisher.PublishAsync(processingStartedMessage, cancellationToken);
        }

        private async Task PublishInvoiceProcessingErrorMessageAsync(int invoiceId, string message, CancellationToken cancellationToken) 
        {
            var errorMessage = new InvoiceProcessingErrorMessage
            {
                InvoiceId = invoiceId,
                Message = message,
                TenantId = applicationContext.TenantId
            };

            await publisher.PublishAsync(errorMessage, cancellationToken);
        }

        private async Task PublishInvoiceRecognitionCompletedMessageAsync(InvoiceReadyForRecognitionMessage message, RecognitionEngineResponse recognitionResult, string fileId, CancellationToken cancellationToken) 
        {
            var invoiceAnalysisCompletedMessage = new InvoiceRecognitionCompletedMessage
            {
                InvoiceId = message.InvoiceId,
                InvoiceProcessingType = InvoiceProcessingType.OCR,
                DataAnnotationFileId = fileId,
                TemplateId = recognitionResult.InvoiceTemplate.Id.ToString(),
                TenantId = message.TenantId,
                TrainingFileCount = recognitionResult.InvoiceTemplate.TrainingFileCount,
                CultureName = message.CultureName
            };

            await publisher.PublishAsync(invoiceAnalysisCompletedMessage, cancellationToken);
        }

        private readonly string annotationsFileName = "annotations.json";
        private readonly IServiceBusPublisher publisher;
        private readonly IRecognitionEngine recognitionEngine;
        private readonly IFieldMapper fieldMapper;
        private readonly IDocumentApiClient documentApiClient;
        private readonly ISerializationService serializationService;
    }
}
