using IdentityServer4.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface IPersistedGrantService : IPersistedGrantStore
    {
        Task RemoveAllExpiredPersistedGrantsAsync(CancellationToken cancelationToken);
    }
}
