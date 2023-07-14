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
    internal class GroupRepository : IGroupRepository
    {
        public GroupRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<int> CreateAsync(Group group, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(group, nameof(group));

            var currentDate = DateTime.UtcNow;

            group.CreatedDate = currentDate;
            group.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(group).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }

            return group.Id;
        }

        public async Task<List<Group>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.Groups
                    .Include(group => group.Tenants)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                return await context.Groups
                    .AnyAsync(group => group.Id == groupId, cancellationToken);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
    }
}
