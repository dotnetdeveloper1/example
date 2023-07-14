using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface ITenantSettingService
    {
        Task<TenantSetting> GetAsync(int tenantId, CancellationToken cancellationToken);
        Task<OperationResult> CreateOrUpdateAsync(int tenantId, int cultureId, CancellationToken cancellationToken);
    }
}
