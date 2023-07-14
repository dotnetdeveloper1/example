using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class InvoiceFieldService : IInvoiceFieldService
    {
        public InvoiceFieldService(IInvoiceFieldRepository invoiceFieldRepository) 
        {
            Guard.IsNotNull(invoiceFieldRepository, nameof(invoiceFieldRepository));

            this.invoiceFieldRepository = invoiceFieldRepository;
        }

        public async Task<InvoiceField> GetAsync(int invoiceFieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceFieldId, nameof(invoiceFieldId));

            return await invoiceFieldRepository.GetAsync(invoiceFieldId, cancellationToken);
        }

        public async Task<List<InvoiceField>> GetListAsync(int invoiceId, CancellationToken cancellationToken)
        {
            return await invoiceFieldRepository.GetListAsync(invoiceId, cancellationToken);
        }

        public async Task CreateAsync(InvoiceField invoiceField, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoiceField, nameof(invoiceField));

            await invoiceFieldRepository.CreateAsync(invoiceField, cancellationToken);
        }

        public async Task CreateAsync(List<InvoiceField> invoiceFields, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoiceFields, nameof(invoiceFields));

            await invoiceFieldRepository.CreateAsync(invoiceFields, cancellationToken);
        }

        public async Task DeleteAsync(int invoiceFieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceFieldId, nameof(invoiceFieldId));
            
            await invoiceFieldRepository.DeleteAsync(invoiceFieldId, cancellationToken);
        }

        public async Task DeleteAsync(List<int> invoiceFieldIds, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(invoiceFieldIds, nameof(invoiceFieldIds));
            foreach (var invoiceFieldId in invoiceFieldIds)
            {
                Guard.IsNotZeroOrNegative(invoiceFieldId, nameof(invoiceFieldIds));
            }

            await invoiceFieldRepository.DeleteAsync(invoiceFieldIds, cancellationToken);
        }

        public async Task UpdateAsync(int invoiceFieldId, InvoiceField invoiceField, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceFieldId, nameof(invoiceFieldId));
            Guard.IsNotNull(invoiceField, nameof(invoiceField));

            await invoiceFieldRepository.UpdateAsync(invoiceFieldId, invoiceField, cancellationToken);
        }

        public async Task UpdateAsync(List<InvoiceField> invoiceFields, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(invoiceFields, nameof(invoiceFields));
            foreach (var invoiceField in invoiceFields)
            {
                Guard.IsNotZeroOrNegative(invoiceField.Id, nameof(invoiceField.Id));
            }

            await invoiceFieldRepository.UpdateAsync(invoiceFields, cancellationToken);
        }

        private readonly IInvoiceFieldRepository invoiceFieldRepository;
    }
}
