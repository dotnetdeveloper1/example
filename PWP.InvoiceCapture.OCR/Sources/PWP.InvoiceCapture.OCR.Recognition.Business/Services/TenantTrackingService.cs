using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    internal class TenantTrackingService : ITenantTrackingService
    {
        public TenantTrackingService(ITenantRepository tenantRepository) 
        {
            Guard.IsNotNull(tenantRepository, nameof(tenantRepository));

            this.tenantRepository = tenantRepository;
        }

        public async Task CreateIfNotExistAsync(string tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive table lock on Tenants to be sure other transactions cannot modify or add new items till this transaction ends.
                await tenantRepository.LockAsync(cancellationToken);

                var exists = await tenantRepository.ExistsAsync(tenantId, cancellationToken);

                if (!exists)
                {
                    await tenantRepository.CreateAsync(tenantId, cancellationToken);
                }

                transaction.Complete();
            }
        }

        public async Task LockAsync(string tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));

            await tenantRepository.LockByTenantIdAsync(tenantId, cancellationToken);
        }

        private readonly ITenantRepository tenantRepository;
    }
}
