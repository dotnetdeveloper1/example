using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Core.DataAccess.Services;

namespace PWP.InvoiceCapture.Core.DataAccess.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInvoicesDatabaseNameProvider(this IServiceCollection services)
        {
            services.AddSingleton<IInvoicesDatabaseNameProvider, InvoicesDatabaseNameProvider>();
        }
    }
}
