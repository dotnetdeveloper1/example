using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.Utilities
{
    public class BackgroundService<TStartup> where TStartup : IServiceStartup, new()
    {
        public BackgroundService()
        {
            startup = new TStartup();
            host = CreateHost();
        }

        public Task StartAsync(CancellationToken cancellationToken) =>
            host.StartAsync(cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await host.StopAsync(cancellationToken);
            await startup.OnShutdownAsync(cancellationToken);
        }

        private IHost CreateHost()
        {
            return new HostBuilder()
                .ConfigureHostConfiguration(startup.ConfigureHostConfiguration)
                .ConfigureAppConfiguration(startup.ConfigureAppConfiguration)
                .ConfigureServices(startup.ConfigureServices)
                .Build();
        }

        private readonly TStartup startup;
        private readonly IHost host;
    }
}
