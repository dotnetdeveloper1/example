using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class PlanRenewalService : IPlanRenewalService
    {
        public PlanRenewalService(
            IGroupPlanRepository groupPlanRepository,
            IGroupRepository groupRepository,
            IPlanService planService)
        {
            Guard.IsNotNull(groupPlanRepository, nameof(groupPlanRepository));
            Guard.IsNotNull(groupRepository, nameof(groupRepository));
            Guard.IsNotNull(planService, nameof(planService));

            
            this.groupPlanRepository = groupPlanRepository;
            this.groupRepository = groupRepository;
            this.planService = planService;
        }
        
        public async Task CheckAndRenewPlansAsync(CancellationToken cancellationToken)
        {
            var groups = await groupRepository.GetListAsync(cancellationToken);
            
            var utcNow = DateTime.UtcNow;
            
            foreach (var group in groups)
            {
                using (var transaction = TransactionManager.Create(IsolationLevel.Serializable, null))
                {
                    
                    var activePlan = await groupPlanRepository.GetActiveAsync(group.Id, utcNow.Date, cancellationToken);
                    
                    if (activePlan!= null && !activePlan.IsRenewalCancelled && CheckExipationDateIsToday(activePlan.EndDate, utcNow.Date))
                    {
                        var newStartDate = activePlan.EndDate.Date.AddDays(1);
                        await planService.CreateGroupPlanAsync(group.Id, activePlan.PlanId, newStartDate, cancellationToken);
                    }
                    
                    transaction.Complete();
                }
            }
        }

        private bool CheckExipationDateIsToday(DateTime endDate, DateTime today)
        {
            return endDate.Date == today;
        }

        private readonly IGroupPlanRepository groupPlanRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IPlanService planService;
    }
}
