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
    internal class PlanRepository : IPlanRepository
    {
        public PlanRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<Plan> GetByIdAsync(int planId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(planId, nameof(planId));

            using (var context = contextFactory.Create())
            {
                return await context.Plans
                    .Include(plan => plan.Currency)
                    .FirstOrDefaultAsync(plan => plan.Id == planId, cancellationToken);
            }
        }

        public async Task<List<Plan>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.Plans
                    .Include(plan => plan.Currency)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<int> CreateAsync(Plan plan, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(plan, nameof(plan));

            var currentDate = DateTime.UtcNow;

            plan.CreatedDate = currentDate;
            plan.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(plan).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }

            return plan.Id;
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
    }
}
