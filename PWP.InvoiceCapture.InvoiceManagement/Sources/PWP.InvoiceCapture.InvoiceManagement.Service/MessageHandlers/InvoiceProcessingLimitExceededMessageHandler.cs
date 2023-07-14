using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.MessageHandlers
{
    internal class InvoiceProcessingLimitExceededMessageHandler : MessageHandlerBase<InvoiceProcessingLimitExceededMessage>
    {
        public InvoiceProcessingLimitExceededMessageHandler(IInvoiceService invoiceService, ITelemetryClient telemetryClient, IApplicationContext applicationContext) 
            : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(invoiceService, nameof(invoiceService));
            this.invoiceService = invoiceService;
        }

        protected override async Task HandleMessageAsync(InvoiceProcessingLimitExceededMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotZeroOrNegative(message.InvoiceId, nameof(message.InvoiceId));

            await invoiceService.UpdateStatusAsync(message.InvoiceId, InvoiceStatus.LimitExceeded, cancellationToken);
            await invoiceService.PublishInvoiceStatusChangedMessageAsync(message.InvoiceId, message.TenantId, cancellationToken);
        }

        private readonly IInvoiceService invoiceService;
    }
}
