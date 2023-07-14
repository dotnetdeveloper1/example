using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using System;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database
{
    internal class ConnectionStringProvider : IConnectionStringProvider
    {
        public ConnectionStringProvider(IInvoicesDatabaseNameProvider databaseNameProvider, IApplicationContext applicationContext, IOptions<DatabaseOptions> optionsAccessor) 
        {
            Guard.IsNotNull(databaseNameProvider, nameof(databaseNameProvider));
            Guard.IsNotNull(applicationContext, nameof(applicationContext));
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            
            this.databaseNameProvider = databaseNameProvider;
            this.applicationContext = applicationContext;
            connectionStringTemplate = GetConnectionStringTemplate(optionsAccessor.Value);
        }

        public string Get()
        {
            var tenantId = applicationContext.TenantId;

            if (tenantId == null)
            {
                throw new InvalidOperationException("TenantId is not specified in Application Context.");
            }

            var databaseName = databaseNameProvider.Get(tenantId);

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new InvalidOperationException("Database name is null or empty string.");
            }

            return string.Format(connectionStringTemplate, databaseName);
        }

        private string GetConnectionStringTemplate(DatabaseOptions databaseOptions) 
        {
            Guard.IsNotNull(databaseOptions, nameof(databaseOptions));
            Guard.IsNotNullOrWhiteSpace(databaseOptions.ConnectionString, nameof(databaseOptions.ConnectionString));

            return databaseOptions.ConnectionString;
        }

        private readonly IInvoicesDatabaseNameProvider databaseNameProvider;
        private readonly IApplicationContext applicationContext;
        private readonly string connectionStringTemplate;
    }
}
