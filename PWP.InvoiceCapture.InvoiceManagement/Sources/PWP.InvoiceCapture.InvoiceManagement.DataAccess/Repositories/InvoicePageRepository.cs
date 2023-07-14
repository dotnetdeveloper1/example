using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories
{
    internal class InvoicePageRepository : IInvoicePageRepository
    {
        public InvoicePageRepository(IDatabaseContextFactory contextFactory) 
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<List<InvoicePage>> GetListAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoicePages
                    .Where(invoicePage => invoicePage.InvoiceId == invoiceId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<InvoicePage> GetAsync(int pageId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(pageId, nameof(pageId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoicePages
                    .FirstOrDefaultAsync(invoicePage => invoicePage.Id == pageId, cancellationToken);
            }
        }

        public async Task CreateAsync(List<InvoicePage> pages, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(pages, nameof(pages));

            using (var context = contextFactory.Create())
            {
                context.InvoicePages.AddRange(pages);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private readonly IDatabaseContextFactory contextFactory;
    }
}
