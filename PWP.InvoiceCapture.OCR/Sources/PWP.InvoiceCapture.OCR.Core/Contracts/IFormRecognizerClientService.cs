using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.Contracts
{
    public interface IFormRecognizerClientService
    {
        Task<IFormRecognizerClient> GetActiveAsync(CancellationToken cancellationToken);
        Task<IFormRecognizerClient> GetAsync(int formRecognizerId, CancellationToken cancellationToken);
        Task DisableAsync(int formRecognizerId, CancellationToken cancellationToken);
    }
}
