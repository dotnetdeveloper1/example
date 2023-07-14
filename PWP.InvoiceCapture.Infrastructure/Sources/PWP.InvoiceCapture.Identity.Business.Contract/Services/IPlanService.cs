using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface IPlanService
    {
        Task<List<GroupPlan>> GetGroupPlanListAsync(int groupId, CancellationToken cancellationToken);
        Task<OperationResult> CreateGroupPlanAsync(int groupId, int  plan, DateTime startDate, CancellationToken cancellationToken);
        Task<Plan> GetByIdAsync(int planId, CancellationToken cancellationToken);
        Task<GroupPlan> GetActiveAsync(int groupId, CancellationToken cancellationToken);
        Task IncreaseCountOfUploadedInvoices(GroupPlan groupPlan, CancellationToken cancellationToken);
        Task<List<Plan>> GetPlanListAsync(CancellationToken cancellationToken);
        Task<OperationResult<int>> CreatePlanAsync(PlanCreationParameters planCreationParameters, CancellationToken cancellationToken);
        Task<OperationResult> DeleteActiveAsync(int groupId, CancellationToken cancellationToken);
        Task<OperationResult> DeleteGroupPlanByIdAsync(int groupPlanId, CancellationToken cancellationToken);
        Task<OperationResult> CancelRenewalAsync(int groupId, CancellationToken cancellationToken);
        Task LockGroupPlansAsync(int groupId, CancellationToken cancellationToken);
    }
}
