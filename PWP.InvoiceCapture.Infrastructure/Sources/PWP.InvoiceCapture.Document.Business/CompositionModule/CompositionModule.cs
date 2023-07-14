using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Document.Business.Contract.Services;
using PWP.InvoiceCapture.Document.Business.Services;

namespace PWP.InvoiceCapture.Document.Business.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddTransient<IDocumentService, DocumentService>();
        }
    }
}
