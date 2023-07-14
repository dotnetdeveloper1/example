using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface ICultureRepository
    {
        Task<List<Culture>> GetListAsync(CancellationToken cancellationToken);
        Task<Culture> GetAsync(int cultureId, CancellationToken cancellationToken);
    }
}
