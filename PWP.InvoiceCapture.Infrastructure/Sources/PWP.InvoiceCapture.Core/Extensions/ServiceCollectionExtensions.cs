using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Services;
using PWP.InvoiceCapture.Core.Telemetry;

namespace PWP.InvoiceCapture.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTelemetryClient(this IServiceCollection services) 
        {
            services.AddTransient<ITelemetryClient, TelemetryClientAdapter>();
        }

        public static void AddEmtpyTelemetryClient(this IServiceCollection services)
        {
            services.AddTransient<ITelemetryClient, EmptyTelemetryClient>();
        }

        public static void AddSerializationService(this IServiceCollection services)
        {
            services.AddTransient<ISerializationService, SerializationService>();
        }

        public static void AddBackgroundServiceContext(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationContext, BackgroundServiceContext>();
        }
    }
}
