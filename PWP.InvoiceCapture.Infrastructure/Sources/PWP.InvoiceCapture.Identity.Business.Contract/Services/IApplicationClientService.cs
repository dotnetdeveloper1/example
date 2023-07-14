using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface IApplicationClientService
    {
        Task<ApplicationClient> GetAsync(string clientId, CancellationToken cancellationToken);
    }
}
