using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IInvoiceFieldService
    {
        Task<InvoiceField> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<InvoiceField>> GetListAsync(int invoiceId, CancellationToken cancellationToken);
        Task CreateAsync(InvoiceField invoiceField, CancellationToken cancellationToken);
        Task CreateAsync(List<InvoiceField> invoiceFields, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task DeleteAsync(List<int> invoiceFieldIds, CancellationToken cancellationToken);
        Task UpdateAsync(int invoiceFieldId, InvoiceField invoiceField, CancellationToken cancellationToken);
        Task UpdateAsync(List<InvoiceField> invoiceFields, CancellationToken cancellationToken);
    }
}
