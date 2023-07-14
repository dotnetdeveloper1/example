using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface ITenantSettingRepository : IEntityLocker
    {
        Task<TenantSetting> GetByTenantIdAsync(int tenantId, CancellationToken cancellationToken);
        Task<int> CreateAsync(TenantSetting tenantSetting, CancellationToken cancellationToken);
        Task UpdateAsync(TenantSetting tenantSettings, CancellationToken cancellationToken);
    }
}
