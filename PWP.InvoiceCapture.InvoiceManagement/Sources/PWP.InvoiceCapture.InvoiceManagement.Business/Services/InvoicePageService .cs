using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class InvoicePageService : IInvoicePageService
    {
        public InvoicePageService(IInvoicePageRepository invoicePageRepository, IDocumentApiClient documentApiClient) 
        {
            Guard.IsNotNull(invoicePageRepository, nameof(invoicePageRepository));
            Guard.IsNotNull(documentApiClient, nameof(documentApiClient));

            this.invoicePageRepository = invoicePageRepository;
            this.documentApiClient = documentApiClient;
        }

        public async Task<List<InvoicePage>> GetListAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            return await invoicePageRepository.GetListAsync(invoiceId, cancellationToken);
        }

        public async Task<string> GetImageLinkAsync(int pageId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(pageId, nameof(pageId));

            var invoicePage = await invoicePageRepository.GetAsync(pageId, cancellationToken);

            if (invoicePage == null)
            {
                return null;
            }

            var imageUrl = await documentApiClient.GetTemporaryLinkAsync(invoicePage.ImageFileId, cancellationToken);

            return imageUrl.Data;
        }

        public Task CreateAsync(List<InvoicePage> pages, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(pages, nameof(pages));

            return invoicePageRepository.CreateAsync(pages, cancellationToken);
        }

        private readonly IInvoicePageRepository invoicePageRepository;
        private readonly IDocumentApiClient documentApiClient;
    }
}
