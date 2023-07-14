using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core
{
    public interface IServiceStartup
    {
        void ConfigureHostConfiguration(IConfigurationBuilder builder);
        void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder);
        void ConfigureServices(IServiceCollection services);
        Task OnShutdownAsync(CancellationToken cancellationToken);
    }
}
