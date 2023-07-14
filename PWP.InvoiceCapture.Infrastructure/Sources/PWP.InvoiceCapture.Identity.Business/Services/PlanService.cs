using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class PlanService : IPlanService
    {
        public PlanService(IPlanRepository planRepository, IGroupPlanRepository groupPlanRepository,
            IGroupRepository groupRepository, ICurrencyRepository currencyRepository)
        {
            Guard.IsNotNull(planRepository, nameof(planRepository));
            Guard.IsNotNull(groupRepository, nameof(groupRepository));
            Guard.IsNotNull(groupPlanRepository, nameof(groupPlanRepository));
            Guard.IsNotNull(currencyRepository, nameof(currencyRepository));

            this.planRepository = planRepository;
            this.groupRepository = groupRepository;
            this.groupPlanRepository = groupPlanRepository;
            this.currencyRepository = currencyRepository;
        }

        public async Task<List<GroupPlan>> GetGroupPlanListAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            return await groupPlanRepository.GetListAsync(groupId, cancellationToken);
        }

        public async Task<OperationResult> CreateGroupPlanAsync(int groupId, int planId, DateTime startDateUtc, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));
            Guard.IsNotZeroOrNegative(planId, nameof(planId));

            if (!await groupRepository.ExistsAsync(groupId, cancellationToken))
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Group with id={groupId} was not found"
                };
            }

            var plan = await planRepository.GetByIdAsync(planId, cancellationToken);

            if (plan == null)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Plan with id={planId} was not found"
                };
            }

            if (startDateUtc == DateTime.MinValue)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"StartDate was not assigned"
                };
            }

            var utcEndDate = CalculatePlanEndDate(plan, startDateUtc);

            if (await groupPlanRepository.IsIntersectAsync(groupId, startDateUtc, utcEndDate, cancellationToken))
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"There is plan that intersects with provided time period."
                };
            }

            var groupPlan = new GroupPlan()
            {
                PlanId = planId,
                GroupId = groupId,
                StartDate = startDateUtc,
                UploadedDocumentsCount = 0,
                EndDate = CalculatePlanEndDate(plan, startDateUtc)
            };

            await groupPlanRepository.CreateAsync(groupPlan, cancellationToken);

            return new OperationResult { Status = OperationResultStatus.Success };
        }

        private DateTime CalculatePlanEndDate(Plan plan, DateTime startDate)
        {
            switch (plan.Type)
            {
                case PlanType.Monthly:
                    return startDate.AddMonths(1);
                case PlanType.Annual:
                    return startDate.AddYears(1);
            }
            return startDate;
        }

        public Task<Plan> GetByIdAsync(int planId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(planId, nameof(planId));

            return planRepository.GetByIdAsync(planId, cancellationToken);
        }

        public Task<GroupPlan> GetActiveAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));
            var now = DateTime.UtcNow.Date;

            return groupPlanRepository.GetActiveAsync(groupId, now, cancellationToken);
        }

        public Task IncreaseCountOfUploadedInvoices(GroupPlan groupPlan, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(groupPlan, nameof(groupPlan));

            groupPlan.UploadedDocumentsCount++;

            return groupPlanRepository.UpdateAsync(groupPlan, cancellationToken);
        }

        public async Task<List<Plan>> GetPlanListAsync(CancellationToken cancellationToken)
        {
            return await planRepository.GetListAsync(cancellationToken);
        }

        public async Task<OperationResult<int>> CreatePlanAsync(PlanCreationParameters planCreationParameters, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(planCreationParameters, nameof(planCreationParameters));

            if (string.IsNullOrWhiteSpace(planCreationParameters.PlanName))
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Name required."
                };
            }

            if (!Enum.IsDefined(typeof(PlanType), planCreationParameters.TypeId))
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Plan type is incorrect."
                };
            }

            if (planCreationParameters.AllowedDocumentsCount < 0)
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.Failed,
                    Message = "AllowedDocumentsCount is less then zero."
                };
            }

            if (!await currencyRepository.ExistsAsync(planCreationParameters.CurrencyId, cancellationToken))
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Currency with id {planCreationParameters.CurrencyId} is not exists."
                };
            }

            var plan = new Plan()
            {
                Name = planCreationParameters.PlanName,
                Type = (PlanType)planCreationParameters.TypeId,
                CurrencyId = planCreationParameters.CurrencyId,
                AllowedDocumentsCount = planCreationParameters.AllowedDocumentsCount,
                Price = planCreationParameters.Price
            };

            var planId = await planRepository.CreateAsync(plan, cancellationToken);

            return new OperationResult<int>
            {
                Data = planId,
                Status = OperationResultStatus.Success,
                Message = $"Plan with id '{planId}' was created."
            };
        }

        public async Task<OperationResult> DeleteActiveAsync(int groupId, CancellationToken cancellationToken)
        {
            var activePlan = await GetActiveAsync(groupId, cancellationToken);
            
            if (activePlan == null)
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.NotFound,
                    Message = $"There is no active plan now for groupId = '{groupId}'."
                };
            }

            await groupPlanRepository.DeleteAsync(activePlan.Id, cancellationToken);

            return OperationResult.Success;
        }

        public async Task<OperationResult> DeleteGroupPlanByIdAsync(int groupPlanId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupPlanId, nameof(groupPlanId));

            var groupPlanToDelete = await groupPlanRepository.GetByIdAsync(groupPlanId, cancellationToken);
            if (groupPlanToDelete == null)
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.NotFound,
                    Message = $"There is no groupPlan with id = '{groupPlanId}'."
                };
            }

            await groupPlanRepository.DeleteAsync(groupPlanToDelete.Id, cancellationToken);
            
            return OperationResult.Success;
        }

        public async Task<OperationResult> CancelRenewalAsync(int groupId, CancellationToken cancellationToken)
        {
            var activePlan = await GetActiveAsync(groupId, cancellationToken);

            if (activePlan == null)
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.NotFound,
                    Message = $"There is no active plan now for groupId = '{groupId}'."
                };
            }

            await groupPlanRepository.CancelRenewalAsync(activePlan.Id, cancellationToken);

            return OperationResult.Success;
        }

        public async Task LockGroupPlansAsync(int groupId, CancellationToken cancellationToken)
        {
            await groupPlanRepository.LockByGroupIdAsync(groupId, cancellationToken);
        }

        private readonly IPlanRepository planRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IGroupPlanRepository groupPlanRepository;
        private readonly ICurrencyRepository currencyRepository;
    }
}
