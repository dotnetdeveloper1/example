using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Samples
{
    public class MessageHandler : MessageHandlerBase<Message>
    {
        public MessageHandler(ITelemetryClient telemetryClient, IApplicationContext applicationContext) : base(telemetryClient, applicationContext)
        { }

        protected override Task HandleMessageAsync(Message message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Console.WriteLine($"MessageHandler: {message.Property}, Message TenantId: {message.TenantId}, Context TenantId: {applicationContext.TenantId}");

            return Task.CompletedTask;
        }
    }
}
