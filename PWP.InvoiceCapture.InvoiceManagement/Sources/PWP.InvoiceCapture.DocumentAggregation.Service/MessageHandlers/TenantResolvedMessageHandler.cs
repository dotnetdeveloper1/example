using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.Service.MessageHandlers
{
    public class TenantEmailResolvedMessageHandler : MessageHandlerBase<TenantEmailResolvedMessage>
    {
        public TenantEmailResolvedMessageHandler(IServiceBusPublisher publisher, ITelemetryClient telemetryClient, IApplicationContext applicationContext) 
            : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(publisher, nameof(publisher));

            this.publisher = publisher;
        }

        protected override async Task HandleMessageAsync(TenantEmailResolvedMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken) 
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotNullOrWhiteSpace(message.FileId, nameof(message.FileId));
            Guard.IsNotNullOrWhiteSpace(message.FileName, nameof(message.FileName));
            Guard.IsNotNullOrWhiteSpace(message.TenantId, nameof(message.TenantId));
            Guard.IsNotNullOrWhiteSpace(message.From, nameof(message.From));
            Guard.IsNotNullOrWhiteSpace(message.CultureName, nameof(message.CultureName));

            await PublishMessageAsync(message, cancellationToken);
        }

        private async Task PublishMessageAsync(TenantEmailResolvedMessage message, CancellationToken cancellationToken)
        {
            var documentUploadedMessage = new InvoiceDocumentUploadedMessage
            {
                FileName = message.FileName,
                FileSourceType = FileSourceType.Email,
                FileId = message.FileId,
                TenantId = applicationContext.TenantId,
                FromEmailAddress = message.From,
                CultureName = message.CultureName
            };

            await publisher.PublishAsync(documentUploadedMessage, cancellationToken);
        }

        private readonly IServiceBusPublisher publisher;
    }
}
