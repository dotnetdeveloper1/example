using Microsoft.Azure.ServiceBus;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.IntegrationTests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal class FakeExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }
    }
}
