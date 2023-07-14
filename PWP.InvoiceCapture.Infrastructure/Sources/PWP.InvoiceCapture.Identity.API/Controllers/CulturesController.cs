using Microsoft.AspNetCore.Mvc;
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
    [Route("cultures")]
    public class CulturesController : ControllerBase
    {
        public CulturesController(ICultureService cultureService)
        {
            Guard.IsNotNull(cultureService, nameof(cultureService));

            this.cultureService = cultureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCulturesListAsync(CancellationToken cancellationToken)
        {
            var cultures = await cultureService.GetListAsync(cancellationToken);

            return Ok(new ApiResponse<List<Culture>> { Data = cultures });
        }

        private readonly ICultureService cultureService;
    }
}
