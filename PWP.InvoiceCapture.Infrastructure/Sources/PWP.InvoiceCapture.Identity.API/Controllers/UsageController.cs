using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.API.Controllers
{
    [ApiController]
    [Route("usage")]
    public class UsageController: ControllerBase
    {
        public UsageController(IUsageService usageService)
        {
            Guard.IsNotNull(usageService, nameof(usageService));

            this.usageService = usageService;
        }

        [HttpGet]
        [Route("groups/{groupId}")]
        public async Task<IActionResult> GetUsageAsync(int groupId, CancellationToken cancellationToken)
        {
            if (groupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(groupId)} parameter has invalid value." });
            }

            var usage = await usageService.GetUsageAsync(groupId, cancellationToken);

            return Ok(
                new ApiResponse<Usage> { Data = usage });
        }

        private readonly IUsageService usageService;
    }
}
