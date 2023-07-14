using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.API.Controllers
{
    [ApiController]
    [Route("tenants")]
    public class TenantsController: ControllerBase
    {
        public TenantsController(ITenantService tenantService)
        {
            Guard.IsNotNull(tenantService, nameof(tenantService));

            this.tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTenantsListAsync(CancellationToken cancellationToken)
        {
            var tenants = await tenantService.GetListAsync(cancellationToken);

            return Ok(
                new ApiResponse<List<Tenant>> { Data = tenants });
        }

        [HttpGet]
        [Route("{tenantId}")]
        public async Task<IActionResult> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken)
        {
            if (tenantId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(tenantId)} parameter has invalid value." });
            }

            var tenant = await tenantService.GetAsync(tenantId, cancellationToken);

            if (tenant == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"Tenant with id = {tenantId} not found." });
            }

            return Ok(
                new ApiResponse<Tenant> { Data = tenant });
        }

        [HttpGet]
        [Route("groups/{groupId}")]
        public async Task<IActionResult> GetListByGroupIdAsync(int groupId, CancellationToken cancellationToken)
        {
            if (groupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(groupId)} parameter has invalid value." });
            }

            var tenants = await tenantService.GetListByGroupIdAsync(groupId, cancellationToken);

            
            return Ok(
                new ApiResponse<List<Tenant>> { Data = tenants });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenantAsync(TenantCreationParameters tenantCreationParameters, CancellationToken cancellationToken)
        {
            var operationResult = await tenantService.CreateAsync(tenantCreationParameters, cancellationToken);

            return operationResult.ToActionResult();
        }

        [Route("clone")]
        [HttpPost]
        public async Task<IActionResult> CloneTenantAsync(int tenantId, string newTenantName, CancellationToken cancellationToken)
        {
            var operationResult = await tenantService.CloneAsync(tenantId, newTenantName, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpPut]
        [Route("{tenantId}/name")]
        public async Task<IActionResult> UpdateTenantNameAsync(int tenantId, string newTenantName, CancellationToken cancellationToken)
        {
            var operationResult = await tenantService.UpdateTenantNameAsync(tenantId, newTenantName, cancellationToken);

            return operationResult.ToActionResult();
        }

        private readonly ITenantService tenantService;
    }
}
