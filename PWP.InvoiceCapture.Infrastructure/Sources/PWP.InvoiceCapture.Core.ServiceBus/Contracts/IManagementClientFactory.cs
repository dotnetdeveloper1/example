using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IManagementClientFactory
    {
        Task<IManagementClient> CreateAsync(string connectionString, CancellationToken cancellationToken);
    }
}
