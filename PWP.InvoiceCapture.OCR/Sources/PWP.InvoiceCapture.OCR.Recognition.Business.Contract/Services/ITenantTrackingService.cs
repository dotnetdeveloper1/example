using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface ITenantTrackingService
    {
        Task CreateIfNotExistAsync(string tenantId, CancellationToken cancellationToken);
        Task LockAsync(string tenantId, CancellationToken cancellationToken);
    }
}
