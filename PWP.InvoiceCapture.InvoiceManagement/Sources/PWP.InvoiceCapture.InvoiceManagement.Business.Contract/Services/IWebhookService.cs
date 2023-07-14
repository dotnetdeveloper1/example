using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IWebhookService
    {
        Task<Webhook> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<Webhook>> GetListAsync(CancellationToken cancellationToken);
        Task<OperationResult> CreateAsync(Webhook invoiceField, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<OperationResult> UpdateAsync(int invoiceFieldId, Webhook webhook, CancellationToken cancellationToken);
        Dictionary<string, int> GetTriggerTypes();
    }
}
