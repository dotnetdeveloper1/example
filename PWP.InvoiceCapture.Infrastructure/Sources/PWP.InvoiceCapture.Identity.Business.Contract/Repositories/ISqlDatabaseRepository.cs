using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface ISqlDatabaseRepository
    {
        TenantDatabaseStatus CreateDatabase(string databaseName, CancellationToken cancellationToken);
        Task<List<SqlDatabase>> GetListAsync(CancellationToken cancellationToken);
    }
}
