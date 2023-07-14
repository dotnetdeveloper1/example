using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface IPersistedGrantRepository
    {
        Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter);
        Task<PersistedGrant> GetAsync(string key);
        Task RemoveAllAsync(PersistedGrantFilter filter);
        Task RemoveAsync(string key);
        Task StoreAsync(PersistedGrant grant);
        Task RemoveAllExpiredAsync(DateTime utcNow, CancellationToken cancelationToken);
    }
}
