using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Repositories
{
    internal class PersistedGrantRepository : IPersistedGrantRepository
    {
        public PersistedGrantRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            using (var context = contextFactory.Create())
            {
                return await context.PersistedGrants.ToListAsync();
            }
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            using (var context = contextFactory.Create())
            {
                return await context.PersistedGrants
                    .FirstOrDefaultAsync(persistedGrant => persistedGrant.Key == key);
            }
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            using (var context = contextFactory.Create())
            {
                var persistedGrants = context.PersistedGrants
                    .Where(persistedGrant => 
                        (filter.ClientId == null || persistedGrant.ClientId == filter.ClientId) &&
                        (filter.SessionId == null || persistedGrant.SessionId == filter.SessionId) &&
                        (filter.SubjectId == null || persistedGrant.SubjectId == filter.SubjectId) &&
                        (filter.Type == null || persistedGrant.Type == filter.Type));

                context.PersistedGrants.RemoveRange(persistedGrants);
                await context.SaveChangesAsync(CancellationToken.None);
            }
        }

        public async Task RemoveAsync(string key)
        {
            using (var context = contextFactory.Create())
            {
                var persistedGrants = context.PersistedGrants
                    .Where(persistedGrant => persistedGrant.Key == key);

                context.PersistedGrants.RemoveRange(persistedGrants);
                await context.SaveChangesAsync(CancellationToken.None);
            }
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            Guard.IsNotNull(grant, nameof(grant));

            using (var context = contextFactory.Create())
            {
                context.Entry(grant).State = EntityState.Added;

                await context.SaveChangesAsync(CancellationToken.None);
            }
        }

        public async Task RemoveAllExpiredAsync(DateTime utcNow, CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                var persistedGrants = context.PersistedGrants
                    .Where(persistedGrant => persistedGrant.Expiration < utcNow);

                context.PersistedGrants.RemoveRange(persistedGrants);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
    }
}
