using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts
{
    internal interface IDocumentAggregationClient
    {
        Task<ApiResponse<UploadDocumentResponse>> UploadFileAsync(Stream content, string fileName, CancellationToken cancellationToken);
    }
}
