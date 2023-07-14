using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface IGroupRepository
    {
        Task<int> CreateAsync(Group group, CancellationToken cancellationToken);
        Task<List<Group>> GetListAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int groupId, CancellationToken cancellationToken);
    }
}
