using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;

namespace PWP.InvoiceCapture.Identity.API.Controllers
{

    [ApiController]
    [Route("tenantSettings")]
    public class TenantSettingsController : ControllerBase
    {
        public TenantSettingsController(ITenantSettingService tenantSettingService)
        {
            Guard.IsNotNull(tenantSettingService, nameof(tenantSettingService));

            this.tenantSettingService = tenantSettingService;
        }

        [HttpGet]
        [Route("tenants/{tenantId}")]
        public async Task<IActionResult> GetByTenantIdAsync(int tenantId, CancellationToken cancellationToken)
        {
            if (tenantId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(tenantId)} parameter has invalid value." });
            }

            var tenantSetting = await tenantSettingService.GetAsync(tenantId, cancellationToken);

            if (tenantSetting == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"TenantSetting with id = {tenantId} not found." });
            }

            return Ok(new ApiResponse<TenantSetting> { Data = tenantSetting });

        }

        [HttpPut]
        [Route("tenants/{tenantId}/cultures/{cultureId}")]
        public async Task<IActionResult> UpdateOrCreateAsync(int tenantId, int cultureId, CancellationToken cancellationToken)
        {
            if (tenantId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(tenantId)} parameter has invalid value." });
            }

            if (cultureId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(cultureId)} parameter has invalid value." });
            }

            var operationResult = await tenantSettingService.CreateOrUpdateAsync(tenantId, cultureId, cancellationToken);

            return operationResult.ToActionResult();
        }

        private readonly ITenantSettingService tenantSettingService;
    }
}
