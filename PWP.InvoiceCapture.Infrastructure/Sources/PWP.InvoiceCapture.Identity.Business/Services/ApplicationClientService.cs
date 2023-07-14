using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class ApplicationClientService : IApplicationClientService
    {
        public ApplicationClientService(IApplicationClientRepository applicationClientRepository)
        {
            Guard.IsNotNull(applicationClientRepository, nameof(applicationClientRepository));
            
            this.applicationClientRepository = applicationClientRepository;
        }

        public Task<ApplicationClient> GetAsync(string clientId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(clientId, nameof(clientId));

            return applicationClientRepository.GetAsync(clientId, cancellationToken);
        }

        private readonly IApplicationClientRepository applicationClientRepository;
    }
}
