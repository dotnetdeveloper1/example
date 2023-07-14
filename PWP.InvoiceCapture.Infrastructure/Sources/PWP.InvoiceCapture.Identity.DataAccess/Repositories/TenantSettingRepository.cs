using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Repositories
{
    internal class TenantSettingRepository : EntityLockerRepositoryBase, ITenantSettingRepository
    {
        public TenantSettingRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<int> CreateAsync(TenantSetting tenantSetting, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(tenantSetting, nameof(tenantSetting));

            var currentDate = DateTime.UtcNow;
            tenantSetting.CreatedDate = currentDate;
            tenantSetting.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(tenantSetting).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }

            return tenantSetting.Id;
        }

        public async Task UpdateAsync(TenantSetting tenantSetting, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(tenantSetting, nameof(tenantSetting));

            tenantSetting.ModifiedDate = DateTime.UtcNow;

            using (var context = contextFactory.Create())
            {
                context.Entry(tenantSetting).State = EntityState.Modified;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<TenantSetting> GetByTenantIdAsync(int tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(tenantId, nameof(tenantId));

            using (var context = contextFactory.Create())
            {
                return await context.TenantSettings
                    .FirstOrDefaultAsync(tenantSetting => tenantSetting.TenantId == tenantId);
            }
        }

        public async Task LockAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                await AquireExclusiveTableLockAsync(context.Database, tableName, cancellationToken);
            }
        }

        public async Task LockAsync(int tenantSettingId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(tenantSettingId, nameof(tenantSettingId));

            using (var context = contextFactory.Create())
            {
                await AquireExclusiveRowLockAsync(context.Database, tenantSettingId, tableName, cancellationToken);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
        private const string tableName = "dbo.TenantSetting";
    }
}
