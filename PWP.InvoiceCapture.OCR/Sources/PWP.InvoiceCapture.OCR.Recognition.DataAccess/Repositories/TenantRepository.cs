using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Repositories
{
    public class TenantRepository : EntityLockerRepositoryBase, ITenantRepository
    {
        public TenantRepository(IRecognitionDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<bool> ExistsAsync(string tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));

            using (var context = contextFactory.Create())
            {
                return await context.Tenants.AnyAsync(tenant => tenant.TenantId == tenantId);
            }
        }

        public async Task CreateAsync(string tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));

            var tenant = new Tenant { TenantId = tenantId };

            using (var context = contextFactory.Create())
            {
                context.Tenants.Add(tenant);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task LockByTenantIdAsync(string tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));

            using (var context = contextFactory.Create())
            {
                var tenant = new Tenant();
                var condition = $"{nameof(tenant.TenantId)} = '{tenantId}'";

                await AquireExclusiveRowLockAsync(context.Database, tableName, condition, cancellationToken);
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

        private readonly IRecognitionDatabaseContextFactory contextFactory;
        private const string tableName = "dbo.Tenants";
    }
}
