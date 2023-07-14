using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories
{
    internal class InvoiceRepository : EntityLockerRepositoryBase, IInvoiceRepository
    {
        public InvoiceRepository(IDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task CreateAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoice, nameof(invoice));

            var currentDate = DateTime.UtcNow;

            invoice.CreatedDate = currentDate;
            invoice.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(invoice).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoice, nameof(invoice));

            invoice.ModifiedDate = DateTime.UtcNow;
            
            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(invoice);

                entityEntry.State = EntityState.Modified;
                entityEntry.Property(entity => entity.CreatedDate).IsModified = false;
                entityEntry.Property(entity => entity.Name).IsModified = false;
                entityEntry.Property(entity => entity.Status).IsModified = false;
                entityEntry.Property(entity => entity.FileId).IsModified = false;
                entityEntry.Property(entity => entity.FileName).IsModified = false;
                entityEntry.Property(entity => entity.FromEmailAddress).IsModified = false;
                entityEntry.Property(entity => entity.FileSourceType).IsModified = false;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateStatusAsync(int invoiceId, InvoiceStatus status, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            var invoice = new Invoice 
            {
                Id = invoiceId,
                Status = status,
                ModifiedDate = DateTime.UtcNow
            };

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(invoice);

                entityEntry.Property(entity => entity.Status).IsModified = true;
                entityEntry.Property(entity => entity.ModifiedDate).IsModified = true;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<Invoice>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                var invoices =  await context.Invoices
                    .Include(invoice => invoice.InvoiceLines)
                    .Include(invoice => invoice.InvoiceFields)
                    .ThenInclude(invoice => invoice.Field)
                    .ToListAsync(cancellationToken);

                return invoices
                    .Select(invoice => RemoveDeletedInvoiceFields(invoice))
                    .ToList();
            }
        }

        public async Task<PaginatedResult<Invoice>> GetPaginatedListAsync(InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(paginatedRequest, nameof(paginatedRequest));

            using (var context = contextFactory.Create())
            {
                var paginatedResult = await context.Invoices
                    .Include(invoice => invoice.InvoiceLines)
                    .Include(invoice => invoice.InvoiceFields)
                    .ThenInclude(invoice => invoice.Field)
                    .ToPaginatedResultAsync(paginatedRequest, cancellationToken);

                return new PaginatedResult<Invoice> 
                {
                    Items = paginatedResult.Items
                        .Select(invoice => RemoveDeletedInvoiceFields(invoice))
                        .ToList(),
                    TotalItemsCount = paginatedResult.TotalItemsCount
                };
            }
        }

        public async Task<List<Invoice>> GetActiveListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                var invoices = await context.Invoices
                    .Include(invoice => invoice.InvoiceLines)
                    .Include(invoice => invoice.InvoiceFields)
                    .ThenInclude(invoice => invoice.Field)
                    .Where(invoice => invoice.InvoiceState == InvoiceState.Active)
                    .ToListAsync(cancellationToken);

                return invoices
                    .Select(invoice => RemoveDeletedInvoiceFields(invoice))
                    .ToList();
            }
        }

        public async Task<PaginatedResult<Invoice>> GetActivePaginatedListAsync(InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(paginatedRequest, nameof(paginatedRequest));

            using (var context = contextFactory.Create())
            {
                var paginatedResult = await context.Invoices
                    .Include(invoice => invoice.InvoiceLines)
                    .Include(invoice => invoice.InvoiceFields)
                    .ThenInclude(invoice => invoice.Field)
                    .Where(invoice => invoice.InvoiceState == InvoiceState.Active)
                    .ToPaginatedResultAsync(paginatedRequest, cancellationToken);

                return new PaginatedResult<Invoice>
                {
                    Items = paginatedResult.Items
                        .Select(invoice => RemoveDeletedInvoiceFields(invoice))
                        .ToList(),
                    TotalItemsCount = paginatedResult.TotalItemsCount
                };
            }
        }

        public async Task<Invoice> GetAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var context = contextFactory.Create())
            {
                var expectedInvoice = await context.Invoices
                    .Include(invoice => invoice.InvoiceLines)
                    .Include(invoice => invoice.InvoiceFields)
                    .ThenInclude(invoice => invoice.Field)
                    .FirstOrDefaultAsync(invoice => invoice.Id == invoiceId, cancellationToken);

                return RemoveDeletedInvoiceFields(expectedInvoice);
            }
        }

        public async Task<Invoice> GetAsync(string fileId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));

            using (var context = contextFactory.Create())
            {
                var expectedInvoice =  await context.Invoices
                    .Include(invoice => invoice.InvoiceLines)
                    .Include(invoice => invoice.InvoiceFields)
                    .ThenInclude(invoice => invoice.Field)
                    .FirstOrDefaultAsync(invoice => invoice.FileId == fileId, cancellationToken);

                return RemoveDeletedInvoiceFields(expectedInvoice);
            }
        }

        public async Task UpdateStateAsync(int invoiceId, InvoiceState state, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            var invoice = new Invoice
            {
                Id = invoiceId,
                InvoiceState = state,
                ModifiedDate = DateTime.UtcNow
            };

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(invoice);

                entityEntry.Property(entity => entity.InvoiceState).IsModified = true;
                entityEntry.Property(entity => entity.ModifiedDate).IsModified = true;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateValidationMessageAsync(int invoiceId, string message, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            var invoice = new Invoice
            {
                Id = invoiceId,
                ValidationMessage = message,
                ModifiedDate = DateTime.UtcNow
            };

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(invoice);

                entityEntry.Property(entity => entity.ValidationMessage).IsModified = true;
                entityEntry.Property(entity => entity.ModifiedDate).IsModified = true;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task LockAsync(int invoiceId, CancellationToken cancellationToken) 
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var context = contextFactory.Create())
            {
                await AquireExclusiveRowLockAsync(context.Database, invoiceId, tableName, cancellationToken);
            }
        }

        public async Task LockAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                await AquireExclusiveTableLockAsync(context.Database, tableName, cancellationToken);
            }
        }

        private Invoice RemoveDeletedInvoiceFields(Invoice invoice)
        {
            if (invoice == null || invoice.InvoiceFields == null)
            {
                return invoice;
            }

            invoice.InvoiceFields = invoice.InvoiceFields
                .Where(invoiceField => invoiceField.Field == null || !invoiceField.Field.IsDeleted)
                .ToList();

            return invoice;
        }

        private readonly IDatabaseContextFactory contextFactory;
        private const string tableName = "dbo.Invoice";
    }
}
