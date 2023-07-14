using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using PWP.InvoiceCapture.Core.Utilities;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Service : StatelessService
    {
        public Service(StatelessServiceContext context)
            : base(context)
        {
            service = new BackgroundService<Startup>();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override Task RunAsync(CancellationToken cancellationToken) => 
            service.StartAsync(cancellationToken);

        protected override Task OnCloseAsync(CancellationToken cancellationToken) =>
            service.StopAsync(cancellationToken);

        private readonly BackgroundService<Startup> service;
    }
}
