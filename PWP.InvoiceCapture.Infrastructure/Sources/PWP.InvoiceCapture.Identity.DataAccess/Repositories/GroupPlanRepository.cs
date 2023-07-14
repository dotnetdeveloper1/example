using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using PWP.InvoiceCapture.Core.Utilities;
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
    internal class GroupPlanRepository : EntityLockerRepositoryBase, IGroupPlanRepository
    {
        public GroupPlanRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task CreateAsync(GroupPlan groupPlan, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(groupPlan, nameof(groupPlan));
            
            var currentDate = DateTime.UtcNow;

            groupPlan.CreatedDate = currentDate;
            groupPlan.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(groupPlan).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<GroupPlan>> GetListAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                return await context.GroupPlans
                    .Include(groupPlan => groupPlan.Plan)
                    .Include(groupPack => groupPack.Plan.Currency)
                    .Where(groupPlan => groupPlan.GroupId == groupId)
                    .ToListAsync(cancellationToken);
            }
        }
      
        public async Task<bool> IsIntersectAsync(int groupId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                return await context.GroupPlans
                    .Where(groupPlan => 
                        groupPlan.GroupId == groupId)
                    .AnyAsync(plan => 
                        (endDate >= plan.StartDate && endDate <= plan.EndDate) ||
                        (startDate >= plan.StartDate && startDate <= plan.EndDate) ||
                        (startDate < plan.StartDate && endDate > plan.EndDate));
            }
        }

        public async Task<GroupPlan> GetActiveAsync(int groupId, DateTime now, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                return await context.GroupPlans
                    .Include(groupPlan => groupPlan.Plan)
                    .Include(groupPlan =>
                        groupPlan.Plan.Currency)
                    .Where(groupPlan =>
                        groupPlan.GroupId == groupId &&
                        groupPlan.StartDate <= now &&
                        groupPlan.EndDate >= now) 
                    .OrderBy(groupPlan => groupPlan.StartDate)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<GroupPlan> GetByIdAsync(int groupPlanId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupPlanId, nameof(groupPlanId));

            using (var context = contextFactory.Create())
            {
                return await context.GroupPlans
                    .Include(groupPlan => groupPlan.Plan)
                    .Include(groupPlan =>
                        groupPlan.Plan.Currency)
                    .FirstOrDefaultAsync(groupPlan =>
                        groupPlan.Id == groupPlanId, cancellationToken);
            }
        }

        public async Task UpdateAsync(GroupPlan groupPlan, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(groupPlan, nameof(groupPlan));
            
            using (var context = contextFactory.Create())
            {
                groupPlan.ModifiedDate = DateTime.UtcNow;
               
                context.Entry(groupPlan).State = EntityState.Modified;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(int groupPlanId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupPlanId, nameof(groupPlanId));

            using (var context = contextFactory.Create())
            {
                var groupPlanToDelete = new GroupPlan()
                {
                    Id = groupPlanId
                };

                context.Entry(groupPlanToDelete).State = EntityState.Deleted;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task CancelRenewalAsync(int groupPlanId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupPlanId, nameof(groupPlanId));

            var planToUpdate = new GroupPlan() { Id = groupPlanId, IsRenewalCancelled = true };
            using (var context = contextFactory.Create())
            {
                planToUpdate.ModifiedDate = DateTime.UtcNow;
                
                context.GroupPlans.Attach(planToUpdate);
                context.Entry(planToUpdate).Property(plan => plan.IsRenewalCancelled).IsModified = true;
                context.Entry(planToUpdate).Property(plan => plan.ModifiedDate).IsModified = true;

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
                var groupPlan = new GroupPlan();
                var condition = $"{nameof(groupPlan.GroupId)} = {groupId}";

                await AquireExclusiveRowLockAsync(context.Database, tableName, condition, cancellationToken);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
        private const string tableName = "dbo.GroupPlan";
    }
}
