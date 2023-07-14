using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Repositories
{
    internal class InvoiceProcessingResultRepository : IInvoiceProcessingResultRepository
    {
        public InvoiceProcessingResultRepository(IInvoicesDatabaseContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<List<InvoiceProcessingResult>> GetCompletedListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                // Assuming there can be only one Processing Result per invoice
                return await context.InvoiceProcessingResults
                    .Include(processingResult => processingResult.Invoice)
                    .Where(processingResult => processingResult.Invoice.Status == InvoiceStatus.Completed)
                    .ToListAsync();
            }
        }

        private readonly IInvoicesDatabaseContextFactory contextFactory;
    }
}
