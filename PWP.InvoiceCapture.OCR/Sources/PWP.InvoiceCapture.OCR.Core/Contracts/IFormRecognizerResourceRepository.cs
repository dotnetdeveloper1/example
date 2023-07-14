using PWP.InvoiceCapture.OCR.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.Contracts
{
    public interface IFormRecognizerResourceRepository
    {
        Task<FormRecognizerResource> GetActiveAsync(CancellationToken cancellationToken);
        Task DisableAsync(int id, CancellationToken cancellationToken);
    }
}
