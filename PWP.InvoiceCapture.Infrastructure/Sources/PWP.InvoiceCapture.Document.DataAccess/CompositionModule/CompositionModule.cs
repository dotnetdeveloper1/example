using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Document.Business.Contract.Repositories;
using PWP.InvoiceCapture.Document.DataAccess.Repositories;

namespace PWP.InvoiceCapture.Document.DataAccess.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddSingleton<IDocumentRepository, AzureBlobRepository>();
        }
    }
}
