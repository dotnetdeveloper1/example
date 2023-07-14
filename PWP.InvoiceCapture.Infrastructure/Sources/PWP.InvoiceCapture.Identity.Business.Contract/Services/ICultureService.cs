using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface ICultureService
    {
        Task<Culture> GetAsync(int cultureId, CancellationToken cancellationToken);   
        Task<List<Culture>> GetListAsync(CancellationToken cancellationToken);
    }
}
