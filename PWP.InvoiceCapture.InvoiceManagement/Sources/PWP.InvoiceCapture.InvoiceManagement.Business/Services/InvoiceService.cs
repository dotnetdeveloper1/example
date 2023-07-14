using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Extensions;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class InvoiceService : IInvoiceService
    {
        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IDocumentApiClient documentApiClient,
            IServiceBusPublisher publisher)
        {
            Guard.IsNotNull(invoiceRepository, nameof(invoiceRepository));
            Guard.IsNotNull(documentApiClient, nameof(documentApiClient));
            Guard.IsNotNull(publisher, nameof(publisher));

            this.invoiceRepository = invoiceRepository;
            this.documentApiClient = documentApiClient;
            this.publisher = publisher;
        }

        public Dictionary<string, int> GetInvoiceStatuses()
        {
            return EnumExtensions.GetKeyValues<InvoiceStatus, int>();
        }

        public Dictionary<string, int> GetInvoiceStates()
        {
            return EnumExtensions.GetKeyValues<InvoiceState, int>();
        }

        public Dictionary<string, int> GetInvoiceProcessingTypes()
        {
            return EnumExtensions.GetKeyValues<InvoiceProcessingType, int>();
        }

        public Dictionary<string, int> GetInvoiceFileSourceTypes()
        {
            return EnumExtensions.GetKeyValues<FileSourceType, int>();
        }

        public Dictionary<string, int> GetInvoiceSortFields()
        {
            return EnumExtensions.GetKeyValues<InvoiceSortField, int>();
        }

        public Dictionary<string, int> GetSortTypes()
        {
            return EnumExtensions.GetKeyValues<SortType, int>();
        }

        public Task CreateAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoice, nameof(invoice));

            return invoiceRepository.CreateAsync(invoice, cancellationToken);
        }

        public Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoice, nameof(invoice));

            return invoiceRepository.UpdateAsync(invoice, cancellationToken);
        }

        public async Task UpdateStatusAsync(int invoiceId, InvoiceStatus status, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            await invoiceRepository.UpdateStatusAsync(invoiceId, status, cancellationToken);
        }

        public async Task<List<Invoice>> GetListAsync(CancellationToken cancellationToken)
        {
            return await invoiceRepository.GetListAsync(cancellationToken);
        }

        public async Task<PaginatedResult<Invoice>> GetPaginatedListAsync(InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(paginatedRequest, nameof(paginatedRequest));

            return await invoiceRepository.GetPaginatedListAsync(paginatedRequest, cancellationToken);
        }

        public async Task<List<Invoice>> GetActiveListAsync(CancellationToken cancellationToken)
        {
            return await invoiceRepository.GetActiveListAsync(cancellationToken);
        }

        public async Task<PaginatedResult<Invoice>> GetActivePaginatedListAsync(InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(paginatedRequest, nameof(paginatedRequest));

            return await invoiceRepository.GetActivePaginatedListAsync(paginatedRequest, cancellationToken);
        }

        public async Task<Invoice> GetAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            return await invoiceRepository.GetAsync(invoiceId, cancellationToken);
        }

        public async Task<Invoice> GetAsync(string documentId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(documentId, nameof(documentId));

            return await invoiceRepository.GetAsync(documentId, cancellationToken);
        }

        public async Task<string> GetDocumentFileLinkAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            var invoice = await invoiceRepository.GetAsync(invoiceId, cancellationToken);

            if (invoice == null)
            {
                return null;
            }

            var response = await documentApiClient.GetTemporaryLinkAsync(invoice.FileId, cancellationToken);

            return response.Data;
        }

        public async Task<OperationResult> UpdateStateAsync(int invoiceId, InvoiceState state, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));
            var invoice = await invoiceRepository.GetAsync(invoiceId, cancellationToken);

            if (invoice == null)
            {
                return OperationResult.NotFound;
            }

            await invoiceRepository.UpdateStateAsync(invoiceId, state, cancellationToken);
            return OperationResult.Success;
        }

        public async Task<OperationResult> RedoAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive row lock on Invoice to be sure other transactions cannot modify it till this transaction ends.
                await invoiceRepository.LockAsync(invoiceId, cancellationToken);

                var invoice = await invoiceRepository.GetAsync(invoiceId, cancellationToken);

                if (invoice == null)
                {
                    return OperationResult.NotFound;
                }

                if (invoice.InvoiceState == InvoiceState.Deleted || invoice.InvoiceState == InvoiceState.Locked)
                {
                    return new OperationResult()
                    {
                        Message = "Redo can't be applied to Invoice in state Deleted or Locked."
                    };
                }

                if (invoice.Status != InvoiceStatus.Completed)
                {
                    return OperationResult.Failed;
                }

                if (invoice.InvoiceState == InvoiceState.Archived)
                {
                    await invoiceRepository.UpdateStateAsync(invoiceId, InvoiceState.Active, cancellationToken);
                }

                await invoiceRepository.UpdateStatusAsync(invoiceId, InvoiceStatus.PendingReview, cancellationToken);

                transaction.Complete();
            }

            return OperationResult.Success;
        }

        public async Task LockAsync(int id, CancellationToken cancellationToken)
        {
            await invoiceRepository.LockAsync(id, cancellationToken);
        }

        public async Task LockAsync(CancellationToken cancellationToken)
        {
            await invoiceRepository.LockAsync(cancellationToken);
        }

        public async Task UpdateValidationMessageAsync(int invoiceId, string validationMessage, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            await invoiceRepository.UpdateValidationMessageAsync(invoiceId, validationMessage, cancellationToken);
        }

        public async Task PublishInvoiceStatusChangedMessageAsync(int invoiceId, string tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));
            Guard.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));

            var message = new InvoiceStatusChangedMessage
            {
                InvoiceId = invoiceId,
                TenantId = tenantId
            };

            await publisher.PublishAsync(message, cancellationToken);
        }

        private readonly IInvoiceRepository invoiceRepository;
        private readonly IDocumentApiClient documentApiClient;
        private readonly IServiceBusPublisher publisher;
    }
}
