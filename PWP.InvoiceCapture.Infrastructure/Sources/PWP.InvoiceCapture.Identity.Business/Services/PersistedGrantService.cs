using IdentityServer4.Models;
using IdentityServer4.Stores;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class PersistedGrantService : IPersistedGrantService
    {
        public PersistedGrantService(IPersistedGrantRepository persistedGrantRepository)
        {
            Guard.IsNotNull(persistedGrantRepository, nameof(persistedGrantRepository));

            this.persistedGrantRepository = persistedGrantRepository;
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            Guard.IsNotNull(filter, nameof(filter));

            return persistedGrantRepository.GetAllAsync(filter);
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            return persistedGrantRepository.GetAsync(key);
        }

        public Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            Guard.IsNotNull(filter, nameof(filter));

            return persistedGrantRepository.RemoveAllAsync(filter);
        }

        public Task RemoveAsync(string key)
        {
            return persistedGrantRepository.RemoveAsync(key);
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            Guard.IsNotNull(grant, nameof(grant));

            return persistedGrantRepository.StoreAsync(grant);
        }

        public Task RemoveAllExpiredPersistedGrantsAsync(CancellationToken cancelationToken)
        {
            var utcNow = DateTime.UtcNow;
            return persistedGrantRepository.RemoveAllExpiredAsync(utcNow, cancelationToken);
        }

        private readonly IPersistedGrantRepository persistedGrantRepository;
    }
}
