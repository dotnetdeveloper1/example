using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface ITenantRepository : IEntityLocker
    {
        Task<int> CreateAsync(Tenant tenant, CancellationToken cancellationToken);
        Task<Tenant> GetIdByUploadEmailAsync(string uploadEmail, CancellationToken cancellationToken);
        Task<List<Tenant>> GetListAsync(CancellationToken cancellationToken);
        Task<List<Tenant>> GetListByGroupIdAsync(int groupId, CancellationToken cancellationToken);
        Task<List<Tenant>> GetListExceptStatusAsync(TenantDatabaseStatus status, CancellationToken cancellationToken);
        Task<Tenant> GetAsync(int tenantId, CancellationToken cancellationToken);
        Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken);
        Task UpdateAsync(List<Tenant> tenants, CancellationToken cancellationToken);
        Task<bool> TenantNameExistsInGroupAsync(string tenantName, int groupId, CancellationToken cancellationToken);
    }
}
