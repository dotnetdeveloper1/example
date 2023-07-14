using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface IGroupPackRepository : IEntityLocker
    {
        Task CreateAsync(GroupPack groupPack, CancellationToken cancellationToken);
        Task<List<GroupPack>> GetListAsync(int groupId, CancellationToken cancellationToken);
        Task<GroupPack> GetByIdAsync(int groupPackId, CancellationToken cancellationToken);
        Task<List<GroupPack>> GetActiveAsync(int groupId, CancellationToken cancellationToken);
        Task UpdateAsync(GroupPack groupPack, CancellationToken cancellationToken);
        Task DeleteAsync(int groupPackId, CancellationToken cancellationToken);
        Task LockByGroupIdAsync(int groupId, CancellationToken cancellationToken);
    }
}
