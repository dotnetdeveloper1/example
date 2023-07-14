using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Repositories
{
    internal class PackRepository : IPackRepository
    {
        public PackRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<Pack> GetByIdAsync(int packId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(packId, nameof(packId));

            using (var context = contextFactory.Create())
            {
                return await context.Packs
                    .Include(pack => pack.Currency)
                    .FirstOrDefaultAsync(pack => pack.Id == packId, cancellationToken);
            }
        }

        public async Task<List<Pack>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.Packs
                    .Include(pack => pack.Currency)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<int> CreateAsync(Pack pack, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(pack, nameof(pack));

            var currentDate = DateTime.UtcNow;

            pack.CreatedDate = currentDate;
            pack.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(pack).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }

            return pack.Id;
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
    }
}
