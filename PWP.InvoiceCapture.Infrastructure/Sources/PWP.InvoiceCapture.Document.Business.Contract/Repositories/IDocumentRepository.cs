using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PWP.InvoiceCapture.Document.Business.Contract.Models;

namespace PWP.InvoiceCapture.Document.Business.Contract.Repositories
{
    public interface IDocumentRepository
    {
        Task<string> SaveAsync(CreateDocumentArgs createDocumentArgs, CancellationToken cancellationToken);
        Task<GetDocumentStreamResult> GetStreamAsync(string fileId, CancellationToken cancellationToken);
        
        Task<string> GetTemporaryLinkAsync(string fileId, CancellationToken cancellationToken);
    }
}
