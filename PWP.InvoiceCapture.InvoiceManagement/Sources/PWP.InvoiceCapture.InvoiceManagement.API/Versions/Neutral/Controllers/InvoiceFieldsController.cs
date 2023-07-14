using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.API.Versions.Neutral.Controllers
{
    [ApiVersionNeutral]
    [ApiController]
    [Route("invoicefields")]
    public class InvoiceFieldsController : ControllerBase
    {
        public InvoiceFieldsController(IInvoiceFieldService invoiceFieldService)
        {
            Guard.IsNotNull(invoiceFieldService, nameof(invoiceFieldService));

            this.invoiceFieldService = invoiceFieldService;
        }

        [HttpGet]
        [Route("{invoiceId}/invoice")]
        public async Task<IActionResult> GetInvoiceFieldListAsync(int invoiceId, CancellationToken cancellationToken)
        {
            var invoiceFields = await invoiceFieldService.GetListAsync(invoiceId, cancellationToken);

            return Ok(
                new ApiResponse<List<InvoiceField>> { Data = invoiceFields });
        }


        [HttpGet]
        [Route("{invoiceFieldId}")]
        public async Task<IActionResult> GetInvoiceFieldByIdAsync(int invoiceFieldId, CancellationToken cancellationToken)
        {
            if (invoiceFieldId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(invoiceFieldId)} parameter has invalid value." });
            }

            var invoiceField = await invoiceFieldService.GetAsync(invoiceFieldId, cancellationToken);

            if (invoiceField == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"InvoiceField with id = {invoiceFieldId} not found." });
            }

            return Ok(
                new ApiResponse<InvoiceField> { Data = invoiceField });
        }

        private readonly IInvoiceFieldService invoiceFieldService;
    }
}
