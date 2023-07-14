using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Database
{
    internal class InvoicesDatabaseConnectionStringProvider : IInvoicesDatabaseConnectionStringProvider
    {
        public InvoicesDatabaseConnectionStringProvider(IInvoicesDatabaseNameProvider databaseNameProvider, IOptions<DatabaseOptions> databaseOptionsAccessor, IOptions<Settings> settingsAccessor) 
        {
            this.databaseNameProvider = databaseNameProvider;
            connectionString = GetConnectionString(databaseOptionsAccessor.Value, settingsAccessor.Value);
        }

        public string Get() => connectionString;

        private string GetConnectionString(DatabaseOptions databaseOptions, Settings settings)
        {
            var tenantId = GetTenantId(settings);
            var databaseName = databaseNameProvider.Get(tenantId);
            
            return string.Format(databaseOptions.ConnectionString, databaseName);
        }

        //TODO: Refactor to use 1 instead of Default in the whole solution, or move this logic to common DatabaseNameProvider (Core project)
        private string GetTenantId(Settings settings) => settings.TenantId == 1 ? "Default" : settings.TenantId.ToString();

        private readonly IInvoicesDatabaseNameProvider databaseNameProvider;
        private readonly string connectionString;
    }
}
