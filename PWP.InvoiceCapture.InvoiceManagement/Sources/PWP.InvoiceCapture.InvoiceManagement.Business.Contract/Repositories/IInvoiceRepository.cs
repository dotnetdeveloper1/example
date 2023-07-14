using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories
{
    public interface IInvoiceRepository : IEntityLocker
    {
        Task CreateAsync(Invoice invoice, CancellationToken cancellationToken);
        Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken);
        Task UpdateStatusAsync(int invoiceId, InvoiceStatus status, CancellationToken cancellationToken);
        Task<List<Invoice>> GetListAsync(CancellationToken cancellationToken);
        Task<PaginatedResult<Invoice>> GetPaginatedListAsync(InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken);
        Task<List<Invoice>> GetActiveListAsync(CancellationToken cancellationToken);
        Task<PaginatedResult<Invoice>> GetActivePaginatedListAsync(InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken);
        Task<Invoice> GetAsync(int invoiceId, CancellationToken cancellationToken);
        Task<Invoice> GetAsync(string documentId, CancellationToken cancellationToken);
        Task UpdateStateAsync(int invoiceId, InvoiceState invoiceState, CancellationToken cancellationToken);
        Task UpdateValidationMessageAsync(int invoiceId, string validationMessage, CancellationToken cancellationToken);
    }
}
