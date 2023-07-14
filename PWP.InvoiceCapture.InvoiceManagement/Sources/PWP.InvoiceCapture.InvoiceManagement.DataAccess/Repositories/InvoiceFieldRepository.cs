using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
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
    internal class InvoiceFieldRepository : IInvoiceFieldRepository
    {
        public InvoiceFieldRepository(IDatabaseContextFactory contextFactory) 
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<List<InvoiceField>> GetListAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceFields
                    .Where(invoiceField => invoiceField.InvoiceId == invoiceId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<InvoiceField> GetAsync(int invoiceFieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceFieldId, nameof(invoiceFieldId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceFields
                    .FirstOrDefaultAsync(invoiceField => invoiceField.Id == invoiceFieldId, cancellationToken);
            }
        }

        public async Task CreateAsync(InvoiceField invoiceField, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoiceField, nameof(invoiceField));

            var currentDate = DateTime.UtcNow;

            invoiceField.CreatedDate = currentDate;
            invoiceField.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.InvoiceFields.Add(invoiceField);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task CreateAsync(List<InvoiceField> invoiceFields, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoiceFields, nameof(invoiceFields));

            var currentDate = DateTime.UtcNow;

            invoiceFields.ForEach(invoiceField =>
            {
                invoiceField.CreatedDate = currentDate;
                invoiceField.ModifiedDate = currentDate;
            });

            using (var context = contextFactory.Create())
            {
                context.InvoiceFields.AddRange(invoiceFields);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var context = contextFactory.Create())
            {
                var invoiceFields = context.InvoiceFields.Where(invoiceField => invoiceField.InvoiceId == invoiceId);

                context.InvoiceFields.RemoveRange(invoiceFields);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));

            using (var context = contextFactory.Create())
            {
                var invoiceField = context.InvoiceFields.FirstOrDefault(field => field.Id == id);

                if (invoiceField != null)
                {
                    context.InvoiceFields.Remove(invoiceField);

                    await context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public async Task DeleteAsync(List<int> invoiceFieldIds, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoiceFieldIds, nameof(invoiceFieldIds));
            foreach (var invoiceFieldId in invoiceFieldIds)
            {
                Guard.IsNotZeroOrNegative(invoiceFieldId, nameof(invoiceFieldId));
            }

            using (var context = contextFactory.Create())
            {
                var invoiceFields = context.InvoiceFields.Where(field => invoiceFieldIds.Any(id => id == field.Id));

                if (invoiceFields.Any())
                {
                    context.InvoiceFields.RemoveRange(invoiceFields);

                    await context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public async Task UpdateAsync(int invoiceFieldId, InvoiceField invoiceField, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceFieldId, nameof(invoiceFieldId));
            Guard.IsNotNull(invoiceField, nameof(invoiceField));

            invoiceField.ModifiedDate = DateTime.UtcNow;
            invoiceField.Id = invoiceFieldId;

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(invoiceField);

                entityEntry.State = EntityState.Modified;
                entityEntry.Property(entity => entity.CreatedDate).IsModified = false;
                entityEntry.Property(entity => entity.InvoiceId).IsModified = false;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateAsync(List<InvoiceField> invoiceFields, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoiceFields, nameof(invoiceFields));
            invoiceFields.ForEach(invoiceField =>
            {
                Guard.IsNotZeroOrNegative(invoiceField.Id, nameof(invoiceField.Id));
            });

            var currentDate = DateTime.UtcNow;

            invoiceFields.ForEach(invoiceField =>
            {
                invoiceField.ModifiedDate = currentDate;
            });

            using (var context = contextFactory.Create())
            {
                foreach (var invoiceField in invoiceFields)
                {
                    var entry = context.Entry(invoiceField);
                    entry.Property(field => field.Value).IsModified = true;
                    entry.Property(field => field.ModifiedDate).IsModified = true;
                }

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private readonly IDatabaseContextFactory contextFactory;
    }
}
