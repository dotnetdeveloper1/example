using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Repositories
{
    internal class ApplicationClientRepository : IApplicationClientRepository
    {
        public ApplicationClientRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<ApplicationClient> GetAsync(string clientId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(clientId, nameof(clientId));

            using (var context = contextFactory.Create())
            {
                return await context.ApplicationClients.FirstOrDefaultAsync(client => client.ClientId == clientId, cancellationToken);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
    }
}
