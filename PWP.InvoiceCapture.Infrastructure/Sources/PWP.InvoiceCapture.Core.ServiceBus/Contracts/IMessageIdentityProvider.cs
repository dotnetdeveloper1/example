using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IMessageIdentityProvider
    {
        Task<string> GetNextAsync(CancellationToken cancellationToken);
    }
}
