using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.MessageHandlers
{
    internal class InvoiceRecognitionCompletedHandler : MessageHandlerBase<InvoiceRecognitionCompletedMessage>
    {
        public InvoiceRecognitionCompletedHandler(IInvoiceProcessingResultService invoiceProcessingResultService, IInvoiceService invoiceService, ITelemetryClient telemetryClient, IApplicationContext applicationContext) 
            : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(invoiceProcessingResultService, nameof(invoiceProcessingResultService));
            Guard.IsNotNull(invoiceService, nameof(invoiceService));

            this.invoiceProcessingResultService = invoiceProcessingResultService;
            this.invoiceService = invoiceService;
        }

        protected override async Task HandleMessageAsync(InvoiceRecognitionCompletedMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotZeroOrNegative(message.InvoiceId, nameof(message.InvoiceId));
            Guard.IsNotNullOrWhiteSpace(message.TemplateId, nameof(message.TemplateId));
            Guard.IsNotNullOrWhiteSpace(message.DataAnnotationFileId, nameof(message.DataAnnotationFileId));

            await CreateInvoiceProcessingResultAsync(message, cancellationToken);
            await invoiceProcessingResultService.ValidateCreatedInvoiceAsync(message.InvoiceId, message.CultureName, cancellationToken);
        }

        private async Task CreateInvoiceProcessingResultAsync(InvoiceRecognitionCompletedMessage message, CancellationToken cancellationToken)
        {
            var processingResult = new InvoiceProcessingResult
            {
                InvoiceId = message.InvoiceId,
                ProcessingType = message.InvoiceProcessingType,
                TemplateId = message.TemplateId,
                DataAnnotationFileId = message.DataAnnotationFileId,
                InitialDataAnnotationFileId = message.DataAnnotationFileId,
                TrainingFileCount = message.TrainingFileCount
            };

            await invoiceProcessingResultService.CreateAsync(processingResult, cancellationToken);
            await invoiceService.PublishInvoiceStatusChangedMessageAsync(message.InvoiceId, message.TenantId, cancellationToken);
        }

        private readonly IInvoiceProcessingResultService invoiceProcessingResultService;
        private readonly IInvoiceService invoiceService;
    }
}
