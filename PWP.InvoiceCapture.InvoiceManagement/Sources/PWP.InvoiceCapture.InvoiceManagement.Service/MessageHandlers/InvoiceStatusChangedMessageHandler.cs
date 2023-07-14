using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.MessageHandlers
{
    internal class InvoiceStatusChangedMessageHandler : MessageHandlerBase<InvoiceStatusChangedMessage>
    {
        public InvoiceStatusChangedMessageHandler(INotificationService notificationService, ITelemetryClient telemetryClient,
            IApplicationContext applicationContext) 
            : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(notificationService, nameof(notificationService));

            this.notificationService = notificationService;
        }

        protected override async Task HandleMessageAsync(InvoiceStatusChangedMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotZeroOrNegative(message.InvoiceId, nameof(message.InvoiceId));
            
            await notificationService.NotifyAsync(message.InvoiceId, cancellationToken);
        }

        private readonly INotificationService notificationService;
    }
}
