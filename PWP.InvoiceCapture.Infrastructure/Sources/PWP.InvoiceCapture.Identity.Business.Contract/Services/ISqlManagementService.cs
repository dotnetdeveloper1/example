using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface ISqlManagementService
    {
        TenantDatabaseStatus CreateSqlDatabase(string databaseName, CancellationToken cancellationToken);
        Task<List<SqlDatabase>> GetSqlDatabasesAsync(CancellationToken cancellationToken);
        Task SetupBackupLongTermPolicesAsync(string dbName, CancellationToken cancellationToken);
    }
}
