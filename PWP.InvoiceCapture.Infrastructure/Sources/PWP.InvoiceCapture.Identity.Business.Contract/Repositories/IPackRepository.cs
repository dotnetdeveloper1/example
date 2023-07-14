using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface IPackRepository
    {
        Task<Pack> GetByIdAsync(int packId, CancellationToken cancellationToken);
        Task<List<Pack>> GetListAsync(CancellationToken cancellationToken);
        Task<int> CreateAsync(Pack pack, CancellationToken cancellationToken);
    }
}
