using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.Contracts
{
    public interface IInvoiceTemplateRepository : IEntityLocker
    {
        Task<List<InvoiceTemplate>> GetAllAsync(string tenantId, CancellationToken cancellationToken);
        Task<List<InvoiceTemplate>> GetAllAsync(CancellationToken cancellationToken);
        Task<InvoiceTemplate> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task UpdateAsync(InvoiceTemplate template, CancellationToken cancellationToken);
        Task UpdateKeyWordCoordinatesAsync(InvoiceTemplate template, CancellationToken cancellationToken);
        Task InsertAsync(InvoiceTemplate template, CancellationToken cancellationToken);
    }
}
