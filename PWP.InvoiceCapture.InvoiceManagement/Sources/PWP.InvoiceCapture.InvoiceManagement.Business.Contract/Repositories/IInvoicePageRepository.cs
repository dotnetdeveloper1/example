using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories
{    
    public interface IInvoicePageRepository
    {
        Task<List<InvoicePage>> GetListAsync(int invoiceId, CancellationToken cancellationToken);
        Task<InvoicePage> GetAsync(int pageId, CancellationToken cancellationToken);
        Task CreateAsync(List<InvoicePage> pages, CancellationToken cancellationToken);
    }
}
