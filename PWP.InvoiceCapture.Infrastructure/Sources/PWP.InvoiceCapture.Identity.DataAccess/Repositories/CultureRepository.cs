using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Repositories
{
    internal class CultureRepository : ICultureRepository
    {
        public CultureRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<List<Culture>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.Cultures.ToListAsync();
            }
        }

        public async Task<Culture> GetAsync(int cultureId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(cultureId, nameof(cultureId));

            using (var context = contextFactory.Create())
            {
                return await context.Cultures.FirstOrDefaultAsync(culture => culture.Id == cultureId);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
    }
}
