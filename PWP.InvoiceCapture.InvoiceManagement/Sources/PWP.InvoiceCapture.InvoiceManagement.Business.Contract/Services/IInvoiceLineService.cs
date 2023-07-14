using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IInvoiceLineService
    {
        Task CreateAsync(List<InvoiceLine> invoiceLines, CancellationToken cancellationToken);
        Task DeleteByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken);
    }
}
