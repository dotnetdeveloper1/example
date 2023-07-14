using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface IApplicationClientRepository
    {
        Task<ApplicationClient> GetAsync(string clientId, CancellationToken cancellationToken);
    }
}
