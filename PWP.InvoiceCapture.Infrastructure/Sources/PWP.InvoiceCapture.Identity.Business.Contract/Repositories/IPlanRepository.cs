using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface IPlanRepository
    {
        Task<Plan> GetByIdAsync(int planId, CancellationToken cancellationToken);
        Task<List<Plan>> GetListAsync(CancellationToken cancellationToken);
        Task<int> CreateAsync(Plan plan, CancellationToken cancellationToken);
    }
}
