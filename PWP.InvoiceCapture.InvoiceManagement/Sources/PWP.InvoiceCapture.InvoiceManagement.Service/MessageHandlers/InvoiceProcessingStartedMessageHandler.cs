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
    internal class InvoiceProcessingStartedMessageHandler : MessageHandlerBase<InvoiceProcessingStartedMessage>
    {
        public InvoiceProcessingStartedMessageHandler(IInvoiceService invoiceService, ITelemetryClient telemetryClient, IApplicationContext applicationContext) 
            : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(invoiceService, nameof(invoiceService));

            this.invoiceService = invoiceService;
        }

        protected override async Task HandleMessageAsync(InvoiceProcessingStartedMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotZeroOrNegative(message.InvoiceId, nameof(message.InvoiceId));

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive row lock on Invoice to be sure other transactions cannot modify it till this transaction ends.
                await invoiceService.LockAsync(message.InvoiceId, cancellationToken);

                var invoice = await invoiceService.GetAsync(message.InvoiceId, cancellationToken);

                if (invoice.Status < InvoiceStatus.InProgress)
                {
                    await invoiceService.UpdateStatusAsync(message.InvoiceId, InvoiceStatus.InProgress, cancellationToken);
                    await invoiceService.PublishInvoiceStatusChangedMessageAsync(message.InvoiceId, message.TenantId, cancellationToken);
                }

                transaction.Complete();
            }
        }

        private readonly IInvoiceService invoiceService;
    }
}
