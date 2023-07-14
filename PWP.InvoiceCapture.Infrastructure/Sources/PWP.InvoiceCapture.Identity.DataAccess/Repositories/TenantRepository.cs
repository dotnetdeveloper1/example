using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
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
    internal class TenantRepository : EntityLockerRepositoryBase, ITenantRepository
    {
        public TenantRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<int> CreateAsync(Tenant tenant, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(tenant, nameof(tenant));

            var currentDate = DateTime.UtcNow;

            tenant.CreatedDate = currentDate;
            tenant.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(tenant).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }

            return tenant.Id;
        }

        public async Task UpdateAsync(Tenant tenant,  CancellationToken cancellationToken)
        {
            Guard.IsNotNull(tenant, nameof(tenant));

            tenant.ModifiedDate = DateTime.UtcNow;

            using (var context = contextFactory.Create())
            {
                context.Entry(tenant).State = EntityState.Modified;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateAsync(List<Tenant> tenants, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(tenants, nameof(tenants));

            var currentDate = DateTime.UtcNow;

            using (var context = contextFactory.Create())
            {
                tenants.ForEach(tenant =>
                {
                    tenant.ModifiedDate = currentDate;
                    context.Entry(tenant).State = EntityState.Modified;
                });

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<Tenant>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.Tenants
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<List<Tenant>> GetListByGroupIdAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                return await context.Tenants
                    .Where(tenant => tenant.GroupId == groupId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<List<Tenant>> GetListExceptStatusAsync(TenantDatabaseStatus status, CancellationToken cancellationToken)
        {
            Guard.IsEnumDefined(status, nameof(status));

            using (var context = contextFactory.Create())
            {
                return await context.Tenants
                    .Where(tenant => tenant.Status != status)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<Tenant> GetAsync(int tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(tenantId, nameof(tenantId));

            using (var context = contextFactory.Create())
            {
                return await context.Tenants
                    .FirstOrDefaultAsync(tenant => tenant.Id == tenantId, cancellationToken);
            }
        }

        public async Task<bool> TenantNameExistsInGroupAsync(string tenantName, int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(tenantName, nameof(tenantName));
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                return await context.Tenants
                    .AnyAsync(tenant => tenant.Name == tenantName && tenant.GroupId == groupId, cancellationToken);
            }
        }

        public async Task<Tenant> GetIdByUploadEmailAsync(string uploadEmail, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(uploadEmail, nameof(uploadEmail));

            using (var context = contextFactory.Create())
            {
                return await context.Tenants
                    .FirstOrDefaultAsync(tenant => tenant.DocumentUploadEmail == uploadEmail.ToLower(), cancellationToken);
            }
        }

        public async Task LockAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                await AquireExclusiveTableLockAsync(context.Database, tableName, cancellationToken);
            }
        }

        public async Task LockAsync(int tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(tenantId, nameof(tenantId));

            using (var context = contextFactory.Create())
            {
                await AquireExclusiveRowLockAsync(context.Database, tenantId, tableName, cancellationToken);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
        private const string tableName = "dbo.Tenant";
    }
}
