using Microsoft.Azure.ServiceBus.Management;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Management
{
    public class DefaultManagementClientFactory : IManagementClientFactory
    {
        public Task<IManagementClient> CreateAsync(string connectionString, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));

            var managementClient = new ManagementClient(connectionString);
            var managementClientAdapter = new ManagementClientAdapter(managementClient);

            return Task.FromResult<IManagementClient>(managementClientAdapter);
        }
    }
}
