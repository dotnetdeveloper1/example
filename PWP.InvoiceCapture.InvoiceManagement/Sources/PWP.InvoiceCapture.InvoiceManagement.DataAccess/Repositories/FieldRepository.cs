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
    internal class FieldRepository : IFieldRepository
    {
        public FieldRepository(IDatabaseContextFactory contextFactory) 
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<Field> GetAsync(int id, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));

            using (var context = contextFactory.Create())
            {
                return await context.Fields
                    .FirstOrDefaultAsync(field => field.Id == id && !field.IsDeleted, cancellationToken);
            }
        }

        public async Task<List<Field>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.Fields
                    .Where(field => !field.IsDeleted)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task CreateAsync(Field field, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(field, nameof(field));

            var currentDate = DateTime.UtcNow;

            field.CreatedDate = currentDate;
            field.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Fields.Add(field);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(int fieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));

            var field = new Field()
            {
                Id = fieldId,
                ModifiedDate = DateTime.UtcNow,
                IsDeleted = true
            };

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(field);
                entityEntry.Property(entity => entity.ModifiedDate).IsModified = true;
                entityEntry.Property(entity => entity.IsDeleted).IsModified = true;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(List<int> fieldIds, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(fieldIds, nameof(fieldIds));
            foreach (var id in fieldIds)
            {
                Guard.IsNotZeroOrNegative(id, nameof(id));
            }

            var currentDate = DateTime.UtcNow;

            var fields = fieldIds.Select(id => new Field() { Id = id, ModifiedDate = currentDate, IsDeleted = true });

            using (var context = contextFactory.Create())
            {
                foreach (var fieldToDelete in fields)
                {
                    var entry = context.Entry(fieldToDelete);
                    entry.Property(field => field.IsDeleted).IsModified = true;
                    entry.Property(field => field.ModifiedDate).IsModified = true;
                }

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateAsync(int fieldId, Field field, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));
            Guard.IsNotNull(field, nameof(field));

            field.ModifiedDate = DateTime.UtcNow;
            field.Id = fieldId;

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(field);

                entityEntry.State = EntityState.Modified;
                entityEntry.Property(entity => entity.CreatedDate).IsModified = false;
                entityEntry.Property(entity => entity.IsProtected).IsModified = false;
                entityEntry.Property(entity => entity.IsDeleted).IsModified = false;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private readonly IDatabaseContextFactory contextFactory;
    }
}
