using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.Services;

namespace PWP.InvoiceCapture.OCR.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInvoiceTemplates(this IServiceCollection services)
        {
            services.AddTransient<IInvoiceTemplateService, InvoiceTemplateService>();
        }
    }
}
