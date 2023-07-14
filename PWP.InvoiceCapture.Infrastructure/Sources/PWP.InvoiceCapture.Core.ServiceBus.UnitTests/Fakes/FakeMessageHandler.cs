using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal class FakeMessageHandler : MessageHandlerBase<FakeMessage>
    {
        public FakeMessageHandler(ITelemetryClient telemetryClient, IApplicationContext applicationContext) : base(telemetryClient, applicationContext) 
        { }

        public FakeMessage ActualInnerMessage { get; private set; }
        public BrokeredMessage ActualBrokeredMessage { get; private set; }

        protected override Task HandleMessageAsync(FakeMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            ActualInnerMessage = message;
            ActualBrokeredMessage = brokeredMessage;

            return Task.CompletedTask;
        }
    }
}
