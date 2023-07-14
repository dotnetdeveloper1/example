using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories
{
    public interface ITenantRepository : IEntityLocker
    {
        Task<bool> ExistsAsync(string tenantId, CancellationToken cancellationToken);
        Task CreateAsync(string tenantId, CancellationToken cancellationToken);
        Task LockByTenantIdAsync(string tenantId, CancellationToken cancellationToken);
    }
}
