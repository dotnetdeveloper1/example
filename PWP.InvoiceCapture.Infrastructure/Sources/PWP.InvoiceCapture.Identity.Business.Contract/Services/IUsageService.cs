using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface IUsageService
    {
        Task<bool> TryIncreaseCountOfUploadedInvoicesAsync(int tenantId, CancellationToken cancellationToken);
        Task<Usage> GetUsageAsync(int groupId, CancellationToken cancellationToken);
    }
}
