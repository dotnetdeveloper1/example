using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Document.API.Client.Contracts;

namespace PWP.InvoiceCapture.Document.API.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDocumentApiClient(this IServiceCollection services) 
        {
            services.AddTransient<IDocumentApiClient, DocumentApiClient>();
        }
    }
}
