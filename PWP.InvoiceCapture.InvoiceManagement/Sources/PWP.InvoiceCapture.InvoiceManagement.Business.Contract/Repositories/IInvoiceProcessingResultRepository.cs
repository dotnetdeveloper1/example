using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories
{
    public interface IInvoiceProcessingResultRepository : IEntityLocker
    {
        Task CreateAsync(InvoiceProcessingResult processingResult, CancellationToken cancellationToken);
        Task<InvoiceProcessingResult> GetAsync(int processingResultId, CancellationToken cancellationToken);
        Task<int?> GetInvoiceIdAsync(int processingResultId, CancellationToken cancellationToken);
        Task<string> GetVendorNameByTemplateIdAsync(string templateId, CancellationToken cancellationToken);
        Task<List<InvoiceProcessingResult>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken);
        Task<InvoiceProcessingResult> GetLastByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken);
        Task UpdateDataAnnotationFileIdAsync(int processingResultId, string dataAnnotationFileId, CancellationToken cancellationToken);
    }
}
