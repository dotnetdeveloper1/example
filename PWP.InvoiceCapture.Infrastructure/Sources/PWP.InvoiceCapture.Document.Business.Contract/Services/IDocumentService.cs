using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Document.Business.Contract.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Document.Business.Contract.Services
{
    public interface IDocumentService
    {
        Task<string> CreateDocumentAsync(Stream fileContent, string fileExtension, CancellationToken cancellationToken);
        Task<OperationResult<GetDocumentStreamResult>> GetDocumentStreamAsync(string fileId, CancellationToken cancellationToken);
        Task<OperationResult<string>> GetTemporaryLinkAsync(string fileId, CancellationToken cancellationToken);
    }
}
