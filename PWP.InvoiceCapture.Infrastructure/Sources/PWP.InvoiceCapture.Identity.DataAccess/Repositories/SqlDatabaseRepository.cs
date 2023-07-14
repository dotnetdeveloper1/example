using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Repositories
{
    internal class SqlDatabaseRepository : ISqlDatabaseRepository
    {
        public SqlDatabaseRepository(IMasterDatabaseContextFactory contextFactory, IOptions<SqlManagementClientOptions> sqlManagementOptionsAccessor)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));
            GuardOptions(sqlManagementOptionsAccessor);

            this.contextFactory = contextFactory;
            options = sqlManagementOptionsAccessor.Value;
        }

        public TenantDatabaseStatus CreateDatabase(string databaseName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(databaseName, nameof(databaseName));

            Task.Run(() => CreateDatabaseAsync(databaseName, cancellationToken));

            return TenantDatabaseStatus.Copying;
        }

        public async Task<List<SqlDatabase>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.Databases.ToListAsync(cancellationToken);
            }
        }

        private void GuardOptions(IOptions<SqlManagementClientOptions> optionsAccessor)
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(optionsAccessor.Value, nameof(optionsAccessor.Value));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.DefaultDatabaseName, nameof(optionsAccessor.Value.DefaultDatabaseName));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.MasterConnectionString, nameof(optionsAccessor.Value.MasterConnectionString));
            Guard.IsNotZeroOrNegative(optionsAccessor.Value.CommandTimeoutInSeconds, nameof(optionsAccessor.Value.CommandTimeoutInSeconds));
        }

        private async Task CreateDatabaseAsync(string databaseName, CancellationToken cancellationToken)
        {
            using (var sqlConnection = new SqlConnection(options.MasterConnectionString))
            {
                var sql = $"CREATE DATABASE {databaseName} AS COPY OF {options.DefaultDatabaseName}";

                using (var sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    sqlCommand.CommandTimeout = options.CommandTimeoutInSeconds;

                    await sqlConnection.OpenAsync(cancellationToken);
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }

        private readonly IMasterDatabaseContextFactory contextFactory;
        private readonly SqlManagementClientOptions options;
    }
}
