using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class InvoiceLineService: IInvoiceLineService
    {
        public InvoiceLineService(IInvoiceLineRepository invoiceLineRepository) 
        {
            Guard.IsNotNull(invoiceLineRepository, nameof(invoiceLineRepository));

            this.invoiceLineRepository = invoiceLineRepository;
        }

        public async Task CreateAsync(List<InvoiceLine> invoiceLines, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(invoiceLines, nameof(invoiceLines));

            await invoiceLineRepository.CreateAsync(invoiceLines, cancellationToken);
        }

        public async Task DeleteByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            await invoiceLineRepository.DeleteByInvoiceIdAsync(invoiceId, cancellationToken);
        }

        private readonly IInvoiceLineRepository invoiceLineRepository;
    }
}
