using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.Service.MessageHandlers
{
    internal class InvoiceCompletedMessageHandler : MessageHandlerBase<InvoiceCompletedMessage>
    {
        public InvoiceCompletedMessageHandler(IServiceBusPublisher publisher, ITelemetryClient telemetryClient, ITemplateManagementService templateManagementService, IApplicationContext applicationContext) : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(templateManagementService, nameof(templateManagementService));
            Guard.IsNotNull(applicationContext, nameof(applicationContext));
            Guard.IsNotNull(publisher, nameof(publisher));

            this.templateManagementService = templateManagementService;
            this.publisher = publisher;
        }

        protected override async Task HandleMessageAsync(InvoiceCompletedMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {   
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotNullOrEmpty(message.DataAnnotationFileId, nameof(message.DataAnnotationFileId));
            Guard.IsNotNullOrEmpty(message.InvoiceFileId, nameof(message.InvoiceFileId));
            Guard.IsNotNullOrEmpty(message.TemplateId, nameof(message.TemplateId));

            await templateManagementService.ProcessUserValidationDataAsync(message.InvoiceId, message.InvoiceFileId, message.DataAnnotationFileId, int.Parse(message.TemplateId), message.TenantId, cancellationToken);
            var dataAnalysisCompletedMessage = new InvoiceDataAnalysisCompletedMessage
            {
                InvoiceId = message.InvoiceId,
                TenantId = message.TenantId
            };

            await publisher.PublishAsync(dataAnalysisCompletedMessage, cancellationToken);
        }

        private readonly ITemplateManagementService templateManagementService;
        private readonly IServiceBusPublisher publisher;
    }
}
