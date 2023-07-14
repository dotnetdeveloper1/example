using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IInvoiceDocumentService
    {
        Task<List<InvoicePage>> GetInvoicePagesAsync(int invoiceId, string fileId, CancellationToken cancellationToken);
    }
}
