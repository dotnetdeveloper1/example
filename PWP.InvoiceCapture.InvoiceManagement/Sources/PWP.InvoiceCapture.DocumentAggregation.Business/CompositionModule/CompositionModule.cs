using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Services;
using PWP.InvoiceCapture.DocumentAggregation.Business.Services;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IInvoiceDocumentService, InvoiceDocumentService>();
        }
    }
}
