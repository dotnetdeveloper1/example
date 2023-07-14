using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories
{
    public class InvoiceTemplateRepository : EntityLockerRepositoryBase, IInvoiceTemplateRepository
    {
        public InvoiceTemplateRepository(IDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task UpdateAsync(InvoiceTemplate template, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(template, nameof(template));

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(template);

                entityEntry.State = EntityState.Modified;
                entityEntry.Reference(entity => entity.GeometricFeatures).IsModified = false;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateKeyWordCoordinatesAsync(InvoiceTemplate template, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(template, nameof(template));

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(template);
                entityEntry.Property(entity => entity.KeyWordCoordinates).IsModified = true;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<InvoiceTemplate>> GetAllAsync(string tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceTemplates
                    .Where(template => template.TenantId == tenantId)
                    .Include(template => template.GeometricFeatures)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<List<InvoiceTemplate>> GetAllAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.InvoiceTemplates
                    .Include(template => template.GeometricFeatures)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<InvoiceTemplate> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceTemplates.FirstOrDefaultAsync(template => template.Id == id, cancellationToken);
            }
        }

        public async Task InsertAsync(InvoiceTemplate template, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(template, nameof(template));

            using (var context = contextFactory.Create())
            {
                context.InvoiceTemplates.Add(template);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task LockAsync(int templateId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(templateId, nameof(templateId));

            using (var context = contextFactory.Create())
            {
                await AquireExclusiveRowLockAsync(context.Database, templateId, tableName, cancellationToken);
            }
        }

        public async Task LockAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                await AquireExclusiveTableLockAsync(context.Database, tableName, cancellationToken);
            }
        }

        private readonly IDatabaseContextFactory contextFactory;
        private const string tableName = "dbo.InvoiceTemplates";
    }
}
