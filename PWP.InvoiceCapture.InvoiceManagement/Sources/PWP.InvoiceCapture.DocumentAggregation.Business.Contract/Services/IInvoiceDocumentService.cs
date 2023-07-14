using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Services
{
    public interface IInvoiceDocumentService
    {
        Task<UploadedDocument> SaveAsync(Stream fileContent, string fileName, FileSourceType sourceType, CancellationToken cancellationToken);
        Task<UploadedDocument> SaveEmailDocumentAsync(string to, string from, string fileName, Stream fileStream, CancellationToken cancellationToken);
    }
}
