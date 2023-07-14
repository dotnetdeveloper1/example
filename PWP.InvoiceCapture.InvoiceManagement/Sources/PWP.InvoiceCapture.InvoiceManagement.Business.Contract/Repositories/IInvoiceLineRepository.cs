using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories
{
    public interface IInvoiceLineRepository
    {
        Task<List<InvoiceLine>> GetListAsync(int invoiceId, CancellationToken cancellationToken);
        Task<InvoiceLine> GetAsync(int invoiceLineId, CancellationToken cancellationToken);
        Task CreateAsync(InvoiceLine invoiceLine, CancellationToken cancellationToken);
        Task CreateAsync(List<InvoiceLine> invoiceLines, CancellationToken cancellationToken);
        Task DeleteByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken);
    }
}
