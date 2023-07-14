using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts
{
    internal interface IInvoiceManagementClient
    {
        Task<List<Invoice>> GetListAsync(CancellationToken cancellationToken);
        Task<string> GetDocumentFileLinkAsync(int invoiceId, CancellationToken cancellationToken);
        Task<Invoice> GetInvoiceByDocumentIdAsync(string documentId, CancellationToken cancellationToken); 
        Task<Invoice> GetInvoiceByIdAsync(int invoiceId, CancellationToken cancellationToken);
        Task<InvoiceProcessingResult> GetLatestProcessingResultByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken);
        Task CompleteAsync(int processingResultId, DataAnnotation dataAnnotation, CancellationToken cancellationToken);
    }
}
