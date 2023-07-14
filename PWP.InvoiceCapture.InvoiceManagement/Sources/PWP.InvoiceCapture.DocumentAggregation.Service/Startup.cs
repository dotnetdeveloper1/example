using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using Microsoft.Extensions.Configuration;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.DocumentAggregation.Service.MessageHandlers;

namespace PWP.InvoiceCapture.DocumentAggregation.Service
{
    public class Startup : ServiceStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            AddMessageHandlers(services);
            AddInvoiceManagementServiceBusPublisher(services);
            StartInvoiceManagementServicBusSubscriber(services);
        }

        protected override void ConfigureOptions(IServiceCollection services)
        {
            base.ConfigureOptions(services);
        }

        public override async Task OnShutdownAsync(CancellationToken cancellationToken)
        {
            await base.OnShutdownAsync(cancellationToken);
            await subscriber?.StopAsync(cancellationToken);
            await publisher?.StopAsync(cancellationToken);
        }

        protected override ICompositionModule[] GetCompositionModules()
        {
            return new ICompositionModule[]
            {
            };
        }

        private void AddMessageHandlers(IServiceCollection services)
        {
            services.AddSingleton<IMessageHandler, TenantEmailResolvedMessageHandler>();
        }

        private void AddInvoiceManagementServiceBusPublisher(IServiceCollection services)
        {
            var factory = new ServiceBusPublisherFactory();
            var options = new ServiceBusPublisherOptions();

            configuration
                .GetSection("InvoiceManagementServiceBusPublisherOptions")
                .Bind(options);

            publisher = factory.Create(options);
            publisher.StartAsync(CancellationToken.None).GetAwaiter().GetResult();

            services.AddSingleton(publisher);
        }

        private void StartInvoiceManagementServicBusSubscriber(IServiceCollection services)
        {
            var factory = new ServiceBusSubscriberFactory();
            var options = new ServiceBusSubscriberOptions();

            configuration
                .GetSection("InvoiceManagementServiceBusSubscriberOptions")
                .Bind(options);

            var serviceProvider = services.BuildServiceProvider();

            var telemetryClient = serviceProvider.GetService<ITelemetryClient>();
            var exceptionHandler = new DefaultExceptionHandler(telemetryClient);

            var messageHandlers = serviceProvider
                .GetServices<IMessageHandler>()
                .ToList();

            subscriber = factory.Create(messageHandlers, exceptionHandler, options);
            subscriber.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        private IServiceBusPublisher publisher;
        private IServiceBusSubscriber subscriber;
    }
}
