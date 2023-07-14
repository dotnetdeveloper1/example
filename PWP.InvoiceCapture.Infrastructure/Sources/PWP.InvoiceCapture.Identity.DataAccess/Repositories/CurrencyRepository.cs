using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Repositories
{
    internal class CurrencyRepository : ICurrencyRepository
    {
        public CurrencyRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<bool> ExistsAsync(int currencyId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(currencyId, nameof(currencyId));

            using (var context = contextFactory.Create())
            {
                return await context.Currencies
                    .AnyAsync(currency => currency.Id == currencyId, cancellationToken);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
    }
}
