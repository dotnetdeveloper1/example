using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories
{
    internal class InvoiceProcessingResultRepository : EntityLockerRepositoryBase, IInvoiceProcessingResultRepository
    {
        public InvoiceProcessingResultRepository(IDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<InvoiceProcessingResult> GetAsync(int processingResultId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(processingResultId, nameof(processingResultId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceProcessingResults
                    .Include(invoiceProcessingResults => invoiceProcessingResults.Invoice)
                    .ThenInclude(invoice => invoice.InvoiceLines)
                    .FirstOrDefaultAsync(invoiceProcessingResults => invoiceProcessingResults.Id == processingResultId, cancellationToken);
            }
        }

        public async Task<int?> GetInvoiceIdAsync(int processingResultId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(processingResultId, nameof(processingResultId));

            using (var context = contextFactory.Create())
            {
                var result = await context.InvoiceProcessingResults
                    .Where(processingResult => processingResult.Id == processingResultId)
                    .Select(processingResult => new { processingResult.InvoiceId })
                    .FirstOrDefaultAsync(cancellationToken);

                return result?.InvoiceId;
            }
        }

        public async Task<List<InvoiceProcessingResult>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceProcessingResults
                    .Include(invoiceProcessingResults => invoiceProcessingResults.Invoice)
                    .ThenInclude(invoice => invoice.InvoiceLines)
                    .Where(invoiceProcessingResults => invoiceProcessingResults.InvoiceId == invoiceId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<InvoiceProcessingResult> GetLastByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceProcessingResults
                    .Include(invoiceProcessingResults => invoiceProcessingResults.Invoice)
                    .ThenInclude(invoice => invoice.InvoiceLines)
                    .Where(invoiceProcessingResults => invoiceProcessingResults.InvoiceId == invoiceId)
                    .OrderByDescending(invoiceProcessingResults => invoiceProcessingResults.CreatedDate)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<string> GetVendorNameByTemplateIdAsync(string templateId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(templateId, nameof(templateId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceProcessingResults
                    .Include(invoiceProcessingResults => invoiceProcessingResults.Invoice)
                    .ThenInclude(invoice => invoice.InvoiceFields)
                    .Where(invoiceProcessingResult =>
                        invoiceProcessingResult.TemplateId == templateId &&
                        invoiceProcessingResult.Invoice.Status == InvoiceStatus.Completed &&
                        invoiceProcessingResult.Invoice.InvoiceFields.Any(field => field.FieldId == FieldTypes.VendorName))
                    .OrderByDescending(invoiceProcessingResults => invoiceProcessingResults.ModifiedDate)
                    .Select(invoiceProcessingResult => 
                        invoiceProcessingResult.Invoice.InvoiceFields.First(field => field.FieldId == FieldTypes.VendorName).Value)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task CreateAsync(InvoiceProcessingResult processingResult, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(processingResult, nameof(processingResult));
            Guard.IsNotZeroOrNegative(processingResult.InvoiceId, nameof(processingResult.InvoiceId));

            var createdDate = DateTime.UtcNow;

            processingResult.CreatedDate = createdDate;
            processingResult.ModifiedDate = createdDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(processingResult).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateDataAnnotationFileIdAsync(int processingResultId, string dataAnnotationFileId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(processingResultId, nameof(processingResultId));
            Guard.IsNotNullOrWhiteSpace(dataAnnotationFileId, nameof(dataAnnotationFileId));

            var processingResult = new InvoiceProcessingResult
            {
                Id = processingResultId,
                DataAnnotationFileId = dataAnnotationFileId,
                ModifiedDate = DateTime.UtcNow
            };

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(processingResult);

                entityEntry.Property(entity => entity.DataAnnotationFileId).IsModified = true;
                entityEntry.Property(entity => entity.ModifiedDate).IsModified = true;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task LockAsync(int processingResultId, CancellationToken cancellationToken) 
        {
            Guard.IsNotZeroOrNegative(processingResultId, nameof(processingResultId));

            using (var context = contextFactory.Create())
            {
                await AquireExclusiveRowLockAsync(context.Database, processingResultId, tableName, cancellationToken);
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
        private const string tableName = "dbo.InvoiceProcessingResult";
    }
}
