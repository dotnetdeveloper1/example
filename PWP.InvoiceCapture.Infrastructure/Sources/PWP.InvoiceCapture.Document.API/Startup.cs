using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Document.Business.Contract.Models;
using StartupBase = PWP.InvoiceCapture.Core.API.StartupBase;

namespace PWP.InvoiceCapture.Document.API
{
    public class Startup : StartupBase
    {
        public Startup(IWebHostEnvironment environment) : base(environment)
        { }

        protected override void ConfigureOptions(IServiceCollection services)
        {
            base.ConfigureOptions(services);

            services.Configure<DocumentStorageOptions>(configuration.GetSection("DocumentStorageOptions"));
        }

        protected override ICompositionModule[] GetCompositionModules()
        {
            return new ICompositionModule[]
            {
                new Business.CompositionModule.CompositionModule(),
                new DataAccess.CompositionModule.CompositionModule()
            };
        }

        protected override void ConfigureMvc(MvcOptions options)
        {
            // Do not register global authorization filter for Document Api
            // Internal services should be accessible without authorization by default
        }

        protected override void AddAuthentication(IServiceCollection services) 
        {
            // Do nothing
            // Internal services should be accessible without authorization by default
        }

        protected override string serviceName => "PWP.InvoiceCapture.Document.API";
        protected override string apiVersion => "v1";
    }
}
