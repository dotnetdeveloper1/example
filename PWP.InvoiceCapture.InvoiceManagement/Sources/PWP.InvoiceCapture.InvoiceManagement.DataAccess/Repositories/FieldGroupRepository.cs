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
    internal class FieldGroupRepository : IFieldGroupRepository
    {
        public FieldGroupRepository(IDatabaseContextFactory contextFactory) 
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<List<FieldGroup>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.FieldGroups
                    .Include(fieldGroup => fieldGroup.Fields)
                    .Where(fieldGroup => !fieldGroup.IsDeleted)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<FieldGroup> GetAsync(int id, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));

            using (var context = contextFactory.Create())
            {
                return await context.FieldGroups
                    .Include(fieldGroup => fieldGroup.Fields)
                    .FirstOrDefaultAsync(fieldGroup => 
                        fieldGroup.Id == id && !fieldGroup.IsDeleted, cancellationToken);
            }
        }

        public async Task CreateAsync(FieldGroup fieldGroup, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(fieldGroup, nameof(fieldGroup));

            var currentDate = DateTime.UtcNow;

            fieldGroup.CreatedDate = currentDate;
            fieldGroup.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.FieldGroups.Add(fieldGroup);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(int fieldGroupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldGroupId, nameof(fieldGroupId));

            var field = new FieldGroup()
            {
                Id = fieldGroupId,
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

        public async Task UpdateAsync(int fieldGroupId, FieldGroup fieldGroup, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldGroupId, nameof(fieldGroupId));
            Guard.IsNotNull(fieldGroup, nameof(fieldGroup));

            fieldGroup.ModifiedDate = DateTime.UtcNow;
            fieldGroup.Id = fieldGroupId;

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(fieldGroup);

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
