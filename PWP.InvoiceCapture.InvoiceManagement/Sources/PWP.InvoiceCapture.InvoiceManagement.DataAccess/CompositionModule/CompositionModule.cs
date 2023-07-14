using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddSingleton<IDatabaseContextFactory, DatabaseContextFactory>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IInvoicePageRepository, InvoicePageRepository>();
            services.AddTransient<IInvoiceLineRepository, InvoiceLineRepository>();
            services.AddTransient<IInvoiceProcessingResultRepository, InvoiceProcessingResultRepository>();
            services.AddTransient<IInvoiceTemplateCultureSettingRepository, InvoiceTemplateCultureSettingRepository>();
            services.AddTransient<IFieldRepository, FieldRepository>();
            services.AddTransient<IFormulaFieldRepository, FormulaFieldRepository>();
            services.AddTransient<IFieldGroupRepository, FieldGroupRepository>();
            services.AddTransient<IInvoiceFieldRepository, InvoiceFieldRepository>();
            services.AddTransient<IWebhookRepository, WebhookRepository>();
        }
    }
}
