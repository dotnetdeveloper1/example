using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface IGroupPlanRepository : IEntityLocker
    {
        Task CreateAsync(GroupPlan groupPlan, CancellationToken cancellationToken);
        Task<GroupPlan> GetActiveAsync(int groupId, DateTime now, CancellationToken cancellationToken);
        Task<List<GroupPlan>> GetListAsync(int groupId, CancellationToken cancellationToken);
        Task<GroupPlan> GetByIdAsync(int groupPlanId, CancellationToken cancellationToken);
        Task<bool> IsIntersectAsync(int groupId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
        Task UpdateAsync(GroupPlan groupPlan, CancellationToken cancellationToken);
        Task DeleteAsync(int groupPlanId, CancellationToken cancellationToken);
        Task CancelRenewalAsync(int groupPlanId, CancellationToken cancellationToken);
        Task LockByGroupIdAsync(int groupId, CancellationToken cancellationToken);
    }
}
