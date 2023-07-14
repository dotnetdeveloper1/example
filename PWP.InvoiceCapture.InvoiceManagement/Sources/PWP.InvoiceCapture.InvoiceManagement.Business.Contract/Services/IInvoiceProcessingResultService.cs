using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IInvoiceProcessingResultService
    {
        Task CreateAsync(InvoiceProcessingResult processingResult, CancellationToken cancellationToken);
        Task<InvoiceProcessingResult> GetAsync(int processingResultId, CancellationToken cancellationToken);
        Task<List<InvoiceProcessingResult>> GetListAsync(int invoiceId, CancellationToken cancellationToken);
        Task<InvoiceProcessingResult> GetLatestAsync(int invoiceId, CancellationToken cancellationToken);
        Task<OperationResult> UpdateDataAnnotationAsync(int processingResultId, UpdatedDataAnnotation updatedDataAnnotation, CancellationToken cancellationToken);
        Task<OperationResult> CompleteAsync(int processingResultId, UpdatedDataAnnotation updatedDataAnnotation, CancellationToken cancellationToken);
        Task ValidateCreatedInvoiceAsync(int invoiceId, string cultureName, CancellationToken cancellationToken);
    }
}
