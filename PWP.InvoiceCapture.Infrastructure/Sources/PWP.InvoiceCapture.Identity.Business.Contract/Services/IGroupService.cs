using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface IGroupService
    {
        Task<List<Group>> GetListAsync(CancellationToken cancellationToken);
        Task<OperationResult<GroupCreationResponse>> CreateAsync(GroupCreationParameters groupCreationParameters, CancellationToken cancellationToken);
    }
}
