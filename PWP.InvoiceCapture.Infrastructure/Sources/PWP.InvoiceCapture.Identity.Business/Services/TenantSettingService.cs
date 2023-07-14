using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    public class TenantSettingService : ITenantSettingService
    {
        public TenantSettingService(ITenantSettingRepository tenantSettingRepository, ICultureRepository cultureRepository)
        {
            Guard.IsNotNull(tenantSettingRepository, nameof(tenantSettingRepository));
            Guard.IsNotNull(cultureRepository, nameof(tenantSettingRepository));

            this.tenantSettingRepository = tenantSettingRepository;
            this.cultureRepository = cultureRepository;
        }

        public async Task<OperationResult> CreateOrUpdateAsync(int tenantId, int cultureId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(tenantId, nameof(tenantId));
            Guard.IsNotZeroOrNegative(cultureId, nameof(cultureId));

            var culture = await cultureRepository.GetAsync(cultureId, cancellationToken);

            if (culture == null)
            {
                return new OperationResult { Status = OperationResultStatus.Failed, Message = $"Culture with id = {cultureId} doesn't exist." };
            }

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive table lock on TenantSettings to be sure other transactions cannot modify or add new items till this transaction ends.
                await tenantSettingRepository.LockAsync(cancellationToken);

                var tenantSetting = await tenantSettingRepository.GetByTenantIdAsync(tenantId, cancellationToken);

                if (tenantSetting == null)
                {
                    tenantSetting = new TenantSetting { TenantId = tenantId, CultureId = cultureId };

                    await tenantSettingRepository.CreateAsync(tenantSetting, cancellationToken);
                }
                else
                {
                    tenantSetting.CultureId = cultureId;

                    await tenantSettingRepository.UpdateAsync(tenantSetting, cancellationToken);
                }

                transaction.Complete();
            }

            return new OperationResult { Status = OperationResultStatus.Success, Message = $"TenantSetting with id = {tenantId} successfully created/updated." };
        }

        public async Task<TenantSetting> GetAsync(int tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(tenantId, nameof(tenantId));

            return await tenantSettingRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        }

        private readonly ITenantSettingRepository tenantSettingRepository;
        private readonly ICultureRepository cultureRepository;
    }
}
