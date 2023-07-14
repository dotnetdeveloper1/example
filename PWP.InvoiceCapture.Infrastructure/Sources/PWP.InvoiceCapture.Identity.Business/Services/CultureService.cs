using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class CultureService : ICultureService
    {
        public CultureService(ICultureRepository cultureRepository)
        {
            Guard.IsNotNull(cultureRepository, nameof(cultureRepository));

            this.cultureRepository = cultureRepository;
        }

        public async Task<Culture> GetAsync(int cultureId, CancellationToken cancellationToken)
        {
            return await cultureRepository.GetAsync(cultureId, cancellationToken);
        }

        public async Task<List<Culture>> GetListAsync(CancellationToken cancellationToken)
        {
            return await cultureRepository.GetListAsync(cancellationToken);
        }

        private readonly ICultureRepository cultureRepository;
    }
}
