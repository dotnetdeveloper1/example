using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface ITenantService
    {
        Task<List<Tenant>> GetListAsync(CancellationToken cancellationToken);
        Task<List<Tenant>> GetListByGroupIdAsync(int groupId, CancellationToken cancellationToken);
        Task<List<Tenant>> GetListExceptStatusAsync(TenantDatabaseStatus status, CancellationToken cancellationToken);
        Task<Tenant> GetAsync(int tenantId, CancellationToken cancellationToken);
        Task<OperationResult<Tenant>> CreateAsync(TenantCreationParameters tenantCreationParameters, CancellationToken cancellationToken);
        Task CheckTenantsDatabasesStateAsync(CancellationToken cancellationToken);
        Task<OperationResult> UpdateTenantNameAsync(int tenantId, string tenantName, CancellationToken cancellationToken);
        Task<OperationResult<Tenant>> CloneAsync(int tenantId, string newTenantName, CancellationToken cancellationToken);
        Task<string> GetTenantIdByEmailAsync(string uploadEmail, CancellationToken cancellationToken);
    }
}
