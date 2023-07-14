using Microsoft.Azure.ServiceBus;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Services
{
    public class DefaultExceptionHandler : IExceptionHandler
    {
        public DefaultExceptionHandler(ITelemetryClient telemetryClient) 
        {
            Guard.IsNotNull(telemetryClient, nameof(telemetryClient));

            this.telemetryClient = telemetryClient;
        }

        public Task HandleAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            telemetryClient.TrackException(exceptionReceivedEventArgs.Exception);

            return Task.CompletedTask;
        }

        private readonly ITelemetryClient telemetryClient;
    }
}
