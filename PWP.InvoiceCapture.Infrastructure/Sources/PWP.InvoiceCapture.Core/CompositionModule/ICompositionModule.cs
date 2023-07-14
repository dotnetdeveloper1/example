using Microsoft.Extensions.DependencyInjection;

namespace PWP.InvoiceCapture.Core.CompositionModule
{
    public interface ICompositionModule
    {
        void RegisterTypes(IServiceCollection services);
    }
}
