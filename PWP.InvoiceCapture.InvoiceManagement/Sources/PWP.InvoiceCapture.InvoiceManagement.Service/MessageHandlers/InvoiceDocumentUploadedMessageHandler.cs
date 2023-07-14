using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.MessageHandlers
{
    internal class InvoiceDocumentUploadedMessageHandler : MessageHandlerBase<InvoiceDocumentUploadedMessage>
    {
        public InvoiceDocumentUploadedMessageHandler(IInvoiceService invoiceService, IServiceBusPublisher publisher, ITelemetryClient telemetryClient, IApplicationContext applicationContext) 
            : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(invoiceService, nameof(invoiceService));
            Guard.IsNotNull(publisher, nameof(publisher));

            this.invoiceService = invoiceService;
            this.publisher = publisher;
        }

        protected override async Task HandleMessageAsync(InvoiceDocumentUploadedMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken) 
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotNullOrWhiteSpace(message.FileId, nameof(message.FileId));
            Guard.IsNotNullOrWhiteSpace(message.FileName, nameof(message.FileName));
            Guard.IsNotNullOrWhiteSpace(message.CultureName, nameof(message.CultureName));

            var invoiceId = await CreateInvoiceAsync(message, cancellationToken);
            await PublishMessageAsync(message, invoiceId, cancellationToken);
        }

        private async Task<int> CreateInvoiceAsync(InvoiceDocumentUploadedMessage message, CancellationToken cancellationToken) 
        {
            var invoice = new Invoice
            {
                FileId = message.FileId,
                FileName = message.FileName,
                Name = Path.GetFileNameWithoutExtension(message.FileName),
                Status = InvoiceStatus.NotStarted,
                FileSourceType = message.FileSourceType,
                InvoiceState = InvoiceState.Active,
                FromEmailAddress = message.FromEmailAddress
            };

            await invoiceService.CreateAsync(invoice, cancellationToken);

            return invoice.Id;
        }

        private async Task PublishMessageAsync(InvoiceDocumentUploadedMessage message, int invoiceId, CancellationToken cancellationToken)
        {
            var invoiceCreatedMessage = new InvoiceCreatedMessage
            {
                FileId = message.FileId,
                InvoiceId = invoiceId,
                TenantId = applicationContext.TenantId,
                CultureName = message.CultureName
            };

            await publisher.PublishAsync(invoiceCreatedMessage, cancellationToken);
        }

        private readonly IInvoiceService invoiceService;
        private readonly IServiceBusPublisher publisher;
    }
}
