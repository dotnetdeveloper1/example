using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface ICurrencyRepository
    {
        Task<bool> ExistsAsync(int currencyId, CancellationToken cancellationToken);
    }
}
