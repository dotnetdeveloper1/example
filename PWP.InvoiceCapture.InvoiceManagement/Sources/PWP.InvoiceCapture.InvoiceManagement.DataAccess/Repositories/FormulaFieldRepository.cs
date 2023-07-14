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
    internal class FormulaFieldRepository : IFormulaFieldRepository
    {
        public FormulaFieldRepository(IDatabaseContextFactory contextFactory) 
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<List<FormulaField>> GetByResultFieldIdAsync(int fieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));

            using (var context = contextFactory.Create())
            {
                return await context.FormulaFields
                    .Where(formulaField => formulaField.ResultFieldId == fieldId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<List<FormulaField>> GetByOperandFieldIdAsync(int fieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));

            using (var context = contextFactory.Create())
            {
                return await context.FormulaFields
                    .Where(formulaField => formulaField.OperandFieldId == fieldId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task CreateAsync(int fieldId, List<int> operandIds, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));
            Guard.IsNotNull(operandIds, nameof(operandIds));

            foreach (var operandId in operandIds)
            {
                Guard.IsNotZeroOrNegative(operandId, nameof(operandId));
            }

            var currentDate = DateTime.UtcNow;

            var formulaFields = operandIds.Select(operandId =>
                new FormulaField()
                {
                    CreatedDate = currentDate,
                    ModifiedDate = currentDate,
                    OperandFieldId = operandId,
                    ResultFieldId = fieldId
                }
            ).ToList();

            using (var context = contextFactory.Create())
            {
                context.FormulaFields.AddRange(formulaFields);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task CreateAsync(int fieldId, int operandId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));
            Guard.IsNotZeroOrNegative(operandId, nameof(operandId));

            var currentDate = DateTime.UtcNow;

            var formulaField = new FormulaField()
            {
                CreatedDate = currentDate,
                ModifiedDate = currentDate,
                OperandFieldId = operandId,
                ResultFieldId = fieldId
            };

            using (var context = contextFactory.Create())
            {
                context.FormulaFields.Add(formulaField);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAllByResultFieldIdAsync(int fieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));

            using (var context = contextFactory.Create())
            {
                var formulaFields = context.FormulaFields.Where(formulaField => formulaField.ResultFieldId == fieldId);

                context.FormulaFields.RemoveRange(formulaFields);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> UsedAsOperandInFormulaAsync(int fieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));

            using (var context = contextFactory.Create())
            {
                return await context.FormulaFields.AnyAsync(formulaField => formulaField.OperandFieldId == fieldId, cancellationToken);
            }
        }

        public async Task<bool> UsedAsResultFieldInFormulaAsync(int fieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));

            using (var context = contextFactory.Create())
            {
                return await context.FormulaFields.AnyAsync(formulaField => formulaField.ResultFieldId == fieldId, cancellationToken);
            }
        }

        private readonly IDatabaseContextFactory contextFactory;
    }
}
