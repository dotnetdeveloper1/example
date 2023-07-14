using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class UsageService : IUsageService
    {
        public UsageService(ITenantService tenantService, IPlanService planService, IPackService packService)
        {
            Guard.IsNotNull(tenantService, nameof(tenantService));
            Guard.IsNotNull(planService, nameof(planService));
            Guard.IsNotNull(packService, nameof(packService));

            this.tenantService = tenantService;
            this.planService = planService;
            this.packService = packService;
        }

        public async Task<bool> TryIncreaseCountOfUploadedInvoicesAsync(int tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(tenantId, nameof(tenantId));
            var tenant = await tenantService.GetAsync(tenantId, cancellationToken);

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive row lock on GroupPlans to be sure other transactions cannot modify or add new items till this transaction ends.
                await planService.LockGroupPlansAsync(tenant.GroupId, cancellationToken);

                var activeGroupPlan = await planService.GetActiveAsync(tenant.GroupId, cancellationToken);

                if (activeGroupPlan != null && activeGroupPlan.UploadedDocumentsCount < activeGroupPlan.Plan.AllowedDocumentsCount)
                {
                    await planService.IncreaseCountOfUploadedInvoices(activeGroupPlan, cancellationToken);
                    transaction.Complete();
                    
                    return true;
                }

                // Aquire exclusive row lock on GroupPack to be sure other transactions cannot modify or add new items till this transaction ends.
                await packService.LockGroupPacksAsync(tenant.GroupId, cancellationToken);

                var activeGroupPacks = await packService.GetActiveGroupPackAsync(tenant.GroupId, cancellationToken);
                
                if (activeGroupPacks.Count > 0)
                {
                    var activeGroupPack = activeGroupPacks[0];
                    await packService.IncreaseCountOfUploadedInvoices(activeGroupPack, cancellationToken);
                    transaction.Complete();
                    
                    return true;
                }

                return false;
            }
        }

        public async Task<Usage> GetUsageAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            var activePlan = await planService.GetActiveAsync(groupId, cancellationToken);
            var activePacks = await packService.GetActiveGroupPackAsync(groupId, cancellationToken);

            var usage = new Usage()
            {
                ActivePlan = activePlan,
                TotalAvailablePacks = activePacks
            };

            return usage;
        }

        private readonly ITenantService tenantService;
        private readonly IPlanService planService;
        private readonly IPackService packService;
    }
}
