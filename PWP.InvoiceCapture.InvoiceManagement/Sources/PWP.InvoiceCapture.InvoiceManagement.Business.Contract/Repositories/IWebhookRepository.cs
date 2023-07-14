using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories
{
    public interface IWebhookRepository
    {
        Task<Webhook> GetAsync(int id, CancellationToken cancellationToken);
        Task<bool> AnyAsync(TriggerType triggerType, string url, CancellationToken cancellationToken);
        Task<bool> AnyAsync(TriggerType triggerType, string url, int excludedId, CancellationToken cancellationToken);
        Task<List<Webhook>> GetListAsync(CancellationToken cancellationToken);
        Task CreateAsync(Webhook webhook, CancellationToken cancellationToken);
        Task UpdateAsync(int id, Webhook webhook, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
