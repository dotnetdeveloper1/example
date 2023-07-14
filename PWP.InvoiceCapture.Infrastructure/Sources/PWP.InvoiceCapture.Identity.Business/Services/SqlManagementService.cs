using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Sql;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class SqlManagementService: ISqlManagementService
    {
        public SqlManagementService(ISqlDatabaseRepository sqlDatabaseRepository, IOptions<LongTermSqlServerBackupOptions> optionsAccessor)
        {
            Guard.IsNotNull(sqlDatabaseRepository, nameof(sqlDatabaseRepository));
            GuardOptions(optionsAccessor);

            this.sqlDatabaseRepository = sqlDatabaseRepository;
            options = optionsAccessor.Value;
        }

        public TenantDatabaseStatus CreateSqlDatabase(string databaseName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(databaseName, nameof(databaseName));

            return sqlDatabaseRepository.CreateDatabase(databaseName, cancellationToken);
        }

        public async Task<List<SqlDatabase>> GetSqlDatabasesAsync(CancellationToken cancellationToken)
        {
            return await sqlDatabaseRepository.GetListAsync(cancellationToken);
        }

        public async Task SetupBackupLongTermPolicesAsync(string dbName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(dbName, nameof(dbName));
            
            if (!options.Enabled)
            {
                return;
            }

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var url = $"https://management.azure.com/";
            var token = await azureServiceTokenProvider.GetAccessTokenAsync(url, true, cancellationToken);
            var tokenCredentials = new TokenCredentials(token);
            var azureCredentials = new AzureCredentials(tokenCredentials, tokenCredentials, options.TenantId, AzureEnvironment.AzureGlobalCloud);
          
            var sqlManagementClient = new SqlManagementClient(azureCredentials);
            sqlManagementClient.SubscriptionId = options.SubscriptionId;
            var retentionPolicy = new Microsoft.Azure.Management.Sql.Models.BackupLongTermRetentionPolicy
            {
                WeeklyRetention = options.WeeklyRetention,
                MonthlyRetention = options.MonthlyRetention,
                YearlyRetention = options.YearlyRetention,
                WeekOfYear = options.WeekOfYear
            };
            await sqlManagementClient.BackupLongTermRetentionPolicies.CreateOrUpdateAsync(options.ResourceGroupName, options.SqlServerName, dbName, retentionPolicy, cancellationToken);
        }

        private void GuardOptions(IOptions<LongTermSqlServerBackupOptions> optionsAccessor)
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(optionsAccessor.Value, nameof(optionsAccessor.Value));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.TenantId, nameof(optionsAccessor.Value.TenantId));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.SubscriptionId, nameof(optionsAccessor.Value.SubscriptionId));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.SqlServerName, nameof(optionsAccessor.Value.SqlServerName));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.WeeklyRetention, nameof(optionsAccessor.Value.WeeklyRetention));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.MonthlyRetention, nameof(optionsAccessor.Value.MonthlyRetention));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.YearlyRetention, nameof(optionsAccessor.Value.YearlyRetention));
            Guard.IsNotZeroOrNegative(optionsAccessor.Value.WeekOfYear, nameof(optionsAccessor.Value.WeekOfYear));
        }

        private readonly ISqlDatabaseRepository sqlDatabaseRepository;
        private readonly LongTermSqlServerBackupOptions options;
    }
}
