using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface INotificationService
    {
        Task NotifyAsync(int invoiceId, CancellationToken cancellationToken);
    }
}
