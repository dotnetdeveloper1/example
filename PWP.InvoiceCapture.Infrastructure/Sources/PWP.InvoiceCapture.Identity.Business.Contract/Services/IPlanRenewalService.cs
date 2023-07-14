using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface IPlanRenewalService
    {
        Task CheckAndRenewPlansAsync(CancellationToken cancellationToken);
    }
}
