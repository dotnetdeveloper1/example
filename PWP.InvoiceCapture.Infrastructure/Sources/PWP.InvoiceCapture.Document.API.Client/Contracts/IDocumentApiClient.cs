using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Document.API.Client.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Document.API.Client.Contracts
{
    public interface IDocumentApiClient
    {
        Task<Stream> GetDocumentStreamAsync(string fileId, CancellationToken cancellationToken);
        Task<ApiResponse<UploadDocumentResponse>> UploadDocumentAsync(Stream content, string fileName, CancellationToken cancellationToken);
        Task<ApiResponse<string>> GetTemporaryLinkAsync(string fileId, CancellationToken cancellationToken);
    }
}
