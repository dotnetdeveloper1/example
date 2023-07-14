using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface IPackService
    {
        Task<List<GroupPack>> GetGroupPackListAsync(int groupId, CancellationToken cancellationToken);
        Task<OperationResult> CreateGroupPackAsync(int groupId, int packId, CancellationToken cancellationToken);
        Task<Pack> GetGroupPackByIdAsync(int packId, CancellationToken cancellationToken);
        Task<List<GroupPack>> GetActiveGroupPackAsync(int groupId, CancellationToken cancellationToken);
        Task IncreaseCountOfUploadedInvoices(GroupPack groupPack, CancellationToken cancellationToken);
        Task<List<Pack>> GetPackListAsync(CancellationToken cancellationToken);
        Task<OperationResult<int>> CreatePackAsync(PackCreationParameters packCreationParameters, CancellationToken cancellationToken);
        Task<OperationResult> DeleteGroupPackByIdAsync(int groupPackId, CancellationToken cancellationToken);
        Task LockGroupPacksAsync(int groupId, CancellationToken cancellationToken);
    }
}
