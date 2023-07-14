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
    internal class InvoiceLineRepository : IInvoiceLineRepository
    {
        public InvoiceLineRepository(IDatabaseContextFactory contextFactory) 
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<List<InvoiceLine>> GetListAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceLines
                    .Where(invoiceLine => invoiceLine.InvoiceId == invoiceId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<InvoiceLine> GetAsync(int invoiceLineId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceLineId, nameof(invoiceLineId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceLines
                    .FirstOrDefaultAsync(invoiceLine => invoiceLine.Id == invoiceLineId, cancellationToken);
            }
        }

        public async Task CreateAsync(InvoiceLine invoiceLine, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoiceLine, nameof(invoiceLine));

            var currentDate = DateTime.UtcNow;

            invoiceLine.CreatedDate = currentDate;
            invoiceLine.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.InvoiceLines.Add(invoiceLine);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task CreateAsync(List<InvoiceLine> invoiceLines, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(invoiceLines, nameof(invoiceLines));

            var currentDate = DateTime.UtcNow;

            invoiceLines.ForEach(invoiceLine => 
            {
                invoiceLine.CreatedDate = currentDate;
                invoiceLine.ModifiedDate = currentDate;
            });

            using (var context = contextFactory.Create())
            {
                context.InvoiceLines.AddRange(invoiceLines);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var context = contextFactory.Create())
            {
                var invoiceLines = context.InvoiceLines
                    .Where(invoiceLine => invoiceLine.InvoiceId == invoiceId);

                context.InvoiceLines.RemoveRange(invoiceLines);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private readonly IDatabaseContextFactory contextFactory;
    }
}
