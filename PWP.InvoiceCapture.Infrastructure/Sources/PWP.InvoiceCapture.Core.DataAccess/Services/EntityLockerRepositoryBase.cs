using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.DataAccess.Services
{
    public abstract class EntityLockerRepositoryBase
    {
        public async Task AquireExclusiveRowLockAsync(DatabaseFacade database, int id, string tableName, CancellationToken cancellationToken)
        {
            using (var command = database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = commandTimeoutInSeconds;
                command.CommandText = $"SELECT TOP 1 1 FROM {tableName} WITH (ROWLOCK, XLOCK) WHERE Id = {id}";

                await database.OpenConnectionAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async Task AquireExclusiveRowLockAsync(DatabaseFacade database, string tableName, string whereCondition, CancellationToken cancellationToken)
        {
            using (var command = database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = commandTimeoutInSeconds;
                command.CommandText = $"SELECT TOP 1 1 FROM {tableName} WITH (ROWLOCK, XLOCK) WHERE {whereCondition}";

                await database.OpenConnectionAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async Task AquireExclusiveTableLockAsync(DatabaseFacade database, string tableName, CancellationToken cancellationToken)
        {
            using (var command = database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = commandTimeoutInSeconds;
                command.CommandText = $"SELECT TOP 1 1 FROM {tableName} WITH (TABLOCKX)";

                await database.OpenConnectionAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        private const int commandTimeoutInSeconds = 120;
    }
}
