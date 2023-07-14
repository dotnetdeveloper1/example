using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using PWP.InvoiceCapture.Identity.Service.MessageHandlers;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using Microsoft.Extensions.Configuration;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using System.Linq;
using PWP.InvoiceCapture.Core.DataAccess.Extensions;

namespace PWP.InvoiceCapture.Identity.Service
{
    public class Startup : ServiceStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            
            services.AddInvoicesDatabaseNameProvider();

            AddMessageHandlers(services);

            AddInvoiceManagementServiceBusPublisher(services);
            StartIdentityServicBusSubscriber(services);
            StartTenantManagementService(services);
        }

        protected override void ConfigureOptions(IServiceCollection services)
        {
            base.ConfigureOptions(services);

            services.Configure<SqlManagementClientOptions>(configuration.GetSection("SqlManagementClientOptions"));
            services.Configure<DatabaseOptions>(configuration.GetSection("TenantsDatabaseOptions"));
            services.Configure<TenantManagementOptions>(configuration.GetSection("TenantManagementOptions"));
            services.Configure<PersistedGrantOptions>(configuration.GetSection("PersistedGrantOptions"));
            services.Configure<EmailAddressGenerationOptions>(configuration.GetSection("EmailAddressGenerationOptions"));
            services.Configure<LongTermSqlServerBackupOptions>(configuration.GetSection("LongTermSqlServerBackupOptions"));
            services.Configure<PlanManagementOptions>(configuration.GetSection("PlanManagementOptions"));
        }

        public override async Task OnShutdownAsync(CancellationToken cancellationToken)
        {
            tenantServiceCancellationTokenSource.Cancel();

            await base.OnShutdownAsync(cancellationToken);
            await subscriber?.StopAsync(cancellationToken);
        }

        protected override ICompositionModule[] GetCompositionModules()
        {
            return new ICompositionModule[]
            {
                new Business.CompositionModule.CompositionModule(),
                new DataAccess.CompositionModule.CompositionModule()
            };
        }

        private void AddMessageHandlers(IServiceCollection services)
        {
            services.AddSingleton<IMessageHandler, InvoiceCreatedMessageHandler>();
            services.AddSingleton<IMessageHandler, EmailDocumentUploadedMessageHandler>();
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

        private void StartTenantManagementService(IServiceCollection services)
        {
            tenantServiceCancellationTokenSource = new CancellationTokenSource();

            var serviceProvider = services.BuildServiceProvider();

            var tenantService = serviceProvider.GetService<ITenantService>();
            var telemetryClient = serviceProvider.GetService<ITelemetryClient>();
            var persistedGrantService = serviceProvider.GetService<IPersistedGrantService>();
            var planRenewalService = serviceProvider.GetService<IPlanRenewalService>();
           
            var tenantManagementOptionsAccessor = serviceProvider.GetService<IOptions<TenantManagementOptions>>();
            var persistedGrantOptionsAccessor = serviceProvider.GetService<IOptions<PersistedGrantOptions>>();
            var planManagementOptionsAccessor = serviceProvider.GetService<IOptions<PlanManagementOptions>>();

            Guard.IsNotNull(tenantService, nameof(tenantService));
            Guard.IsNotNull(tenantService, nameof(tenantService));
            Guard.IsNotNull(planRenewalService, nameof(planRenewalService));

            GuardTenantManagementOptions(tenantManagementOptionsAccessor);
            GuardPersistedGrantOptions(persistedGrantOptionsAccessor);
            GuardPlanManagementOptions(planManagementOptionsAccessor);

            Task.Run(() => StartLoopAsync(
                tenantManagementOptionsAccessor.Value.TenantsDatabaseCheckIntervalInSeconds,
                telemetryClient,
                tenantService.CheckTenantsDatabasesStateAsync, 
                tenantServiceCancellationTokenSource.Token
            ));

            Task.Run(() => StartLoopAsync(
                persistedGrantOptionsAccessor.Value.PersistedGrantOptionsCheckIntervalInSeconds,
                telemetryClient,
                persistedGrantService.RemoveAllExpiredPersistedGrantsAsync,
                tenantServiceCancellationTokenSource.Token
            ));

            Task.Run(() => StartLoopAsync(
                planManagementOptionsAccessor.Value.RenewalCheckIntervalInSeconds,
                TimeSpan.Parse(planManagementOptionsAccessor.Value.RenewalTimeUtc),
                telemetryClient,
                planRenewalService.CheckAndRenewPlansAsync,
                tenantServiceCancellationTokenSource.Token
            ));
        }

        private async Task StartLoopAsync(
            int delayInSeconds,
            ITelemetryClient telemetryClient,
            Func<CancellationToken, Task> action,
            CancellationToken cancellationToken)
        { 
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await action(cancellationToken);
                }
                catch (Exception exception)
                {
                    telemetryClient.TrackException(exception);
                }

                await Task.Delay(TimeSpan.FromSeconds(delayInSeconds), cancellationToken);
            }
        }

        private async Task StartLoopAsync(
           int delayInSeconds,
           TimeSpan timeOfTheDay,
           ITelemetryClient telemetryClient,
           Func<CancellationToken, Task> action,
           CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (DateTime.UtcNow.TimeOfDay >= timeOfTheDay)
                {
                    try
                    {
                        await action(cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        telemetryClient.TrackException(exception);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(delayInSeconds), cancellationToken);
            }
        }

        private void GuardTenantManagementOptions(IOptions<TenantManagementOptions> tenantManagementOptionsAccessor)
        {
            Guard.IsNotNull(tenantManagementOptionsAccessor, nameof(tenantManagementOptionsAccessor));
            Guard.IsNotNull(tenantManagementOptionsAccessor.Value, nameof(tenantManagementOptionsAccessor.Value));
            Guard.IsNotZeroOrNegative(
                tenantManagementOptionsAccessor.Value.TenantsDatabaseCheckIntervalInSeconds,
                nameof(tenantManagementOptionsAccessor.Value.TenantsDatabaseCheckIntervalInSeconds));
        }

        private void GuardPersistedGrantOptions(IOptions<PersistedGrantOptions> persistedGrantOptionsAccessor)
        {
            Guard.IsNotNull(persistedGrantOptionsAccessor, nameof(persistedGrantOptionsAccessor));
            Guard.IsNotNull(persistedGrantOptionsAccessor.Value, nameof(persistedGrantOptionsAccessor.Value));
            Guard.IsNotZeroOrNegative(
                persistedGrantOptionsAccessor.Value.PersistedGrantOptionsCheckIntervalInSeconds,
                nameof(persistedGrantOptionsAccessor.Value.PersistedGrantOptionsCheckIntervalInSeconds));
        }

        private void GuardPlanManagementOptions(IOptions<PlanManagementOptions> planManagementOptionsAccessor)
        {
            Guard.IsNotNull(planManagementOptionsAccessor, nameof(planManagementOptionsAccessor));
            Guard.IsNotNull(planManagementOptionsAccessor.Value, nameof(planManagementOptionsAccessor.Value));
            Guard.IsNotZeroOrNegative(
                planManagementOptionsAccessor.Value.RenewalCheckIntervalInSeconds,
                nameof(planManagementOptionsAccessor.Value.RenewalCheckIntervalInSeconds));
            Guard.IsNotNullOrWhiteSpace(
                planManagementOptionsAccessor.Value.RenewalTimeUtc,
                nameof(planManagementOptionsAccessor.Value.RenewalTimeUtc));
        }

        private void StartIdentityServicBusSubscriber(IServiceCollection services)
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

        private CancellationTokenSource tenantServiceCancellationTokenSource;
        private IServiceBusSubscriber subscriber;
        private IServiceBusPublisher publisher;
    }
}
