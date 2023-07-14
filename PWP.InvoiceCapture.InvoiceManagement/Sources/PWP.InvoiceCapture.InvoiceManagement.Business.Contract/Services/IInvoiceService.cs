using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IInvoiceService : IEntityLocker
    {
        Dictionary<string, int> GetInvoiceStatuses();
        Dictionary<string, int> GetInvoiceStates();
        Dictionary<string, int> GetInvoiceProcessingTypes();
        Dictionary<string, int> GetInvoiceFileSourceTypes();
        Dictionary<string, int> GetInvoiceSortFields();
        Dictionary<string, int> GetSortTypes();
        Task CreateAsync(Invoice invoice, CancellationToken cancellationToken);
        Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken);
        Task UpdateStatusAsync(int invoiceId, InvoiceStatus status, CancellationToken cancellationToken);
        Task<List<Invoice>> GetListAsync(CancellationToken cancellationToken);
        Task<PaginatedResult<Invoice>> GetPaginatedListAsync(InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken);
        Task<List<Invoice>> GetActiveListAsync(CancellationToken cancellationToken);
        Task<PaginatedResult<Invoice>> GetActivePaginatedListAsync(InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken);
        Task<Invoice> GetAsync(int invoiceId, CancellationToken cancellationToken);
        Task<string> GetDocumentFileLinkAsync(int invoiceId, CancellationToken cancellationToken);
        Task<Invoice> GetAsync(string documentId, CancellationToken cancellationToken);
        Task<OperationResult> UpdateStateAsync(int invoiceId, InvoiceState invoiceState, CancellationToken cancellationToken);
        Task<OperationResult> RedoAsync(int invoiceId, CancellationToken cancellationToken);
        Task UpdateValidationMessageAsync(int invoiceId, string message, CancellationToken cancellationToken);
        Task PublishInvoiceStatusChangedMessageAsync(int invoiceId, string tenantId, CancellationToken cancellationToken);
    }
}
