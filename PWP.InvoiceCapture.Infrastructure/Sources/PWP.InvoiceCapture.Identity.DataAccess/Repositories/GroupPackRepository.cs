using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Repositories
{
    internal class GroupPackRepository : EntityLockerRepositoryBase, IGroupPackRepository
    {
        public GroupPackRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task CreateAsync(GroupPack groupPack, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(groupPack, nameof(groupPack));

            var currentDate = DateTime.UtcNow;

            groupPack.CreatedDate = currentDate;
            groupPack.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(groupPack).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<GroupPack>> GetListAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                return await context.GroupPacks
                    .Include(groupPack => groupPack.Pack)
                    .Include(groupPack => groupPack.Pack.Currency)
                    .Where(groupPack => groupPack.GroupId == groupId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<List<GroupPack>> GetActiveAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                return await context.GroupPacks
                    .Include(groupPack => groupPack.Pack)
                    .Include(groupPack => groupPack.Pack.Currency)
                    .Where(groupPack =>
                        groupPack.GroupId == groupId && 
                        groupPack.UploadedDocumentsCount < groupPack.Pack.AllowedDocumentsCount)
                    .OrderBy(groupPack => groupPack.CreatedDate)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<GroupPack> GetByIdAsync(int groupPackId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupPackId, nameof(groupPackId));

            using (var context = contextFactory.Create())
            {
                return await context.GroupPacks
                    .Include(groupPack => groupPack.Pack)
                    .Include(groupPack =>
                        groupPack.Pack.Currency)
                    .FirstOrDefaultAsync(groupPack =>
                        groupPack.Id == groupPackId, cancellationToken);
            }
        }

        public async Task DeleteAsync(int groupPackId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupPackId, nameof(groupPackId));

            using (var context = contextFactory.Create())
            {
                var groupPackToDelete = new GroupPack()
                {
                    Id = groupPackId
                };

                context.Entry(groupPackToDelete).State = EntityState.Deleted;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateAsync(GroupPack groupPack, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(groupPack, nameof(groupPack));

            using (var context = contextFactory.Create())
            {
                groupPack.ModifiedDate = DateTime.UtcNow;
                
                context.Entry(groupPack).State = EntityState.Modified;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task LockAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                await AquireExclusiveTableLockAsync(context.Database, tableName, cancellationToken);
            }
        }

        public async Task LockAsync(int id, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));

            using (var context = contextFactory.Create())
            {
                await AquireExclusiveRowLockAsync(context.Database, id, tableName, cancellationToken);
            }
        }

        public async Task LockByGroupIdAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                var groupPlan = new GroupPack();
                var condition = $"{nameof(groupPlan.GroupId)} = {groupId}";

                await AquireExclusiveRowLockAsync(context.Database, tableName, condition, cancellationToken);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
        private const string tableName = "dbo.GroupPack";
    }
}
