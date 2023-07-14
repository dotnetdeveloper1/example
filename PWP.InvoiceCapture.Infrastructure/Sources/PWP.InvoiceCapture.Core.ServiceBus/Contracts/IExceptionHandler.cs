using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IExceptionHandler
    {
        Task HandleAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs);
    }
}
