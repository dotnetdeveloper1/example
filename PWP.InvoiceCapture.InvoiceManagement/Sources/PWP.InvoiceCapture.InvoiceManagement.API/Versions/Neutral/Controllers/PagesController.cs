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
    [Route("pages")]
    public class PagesController : ControllerBase
    {
        public PagesController(IInvoicePageService invoicePageService)
        {
            Guard.IsNotNull(invoicePageService, nameof(invoicePageService));

            this.invoicePageService = invoicePageService;
        }

        [HttpGet("invoices/{invoiceId}")]
        public async Task<IActionResult> GetInvoicePagesAsync(int invoiceId, CancellationToken cancellationToken)
        {
            if (invoiceId < 1)
            {
                return BadRequest(
                    new ApiResponse { Message = "InvoiceId should be greater then 0." });
            }

            var pages = await invoicePageService.GetListAsync(invoiceId, cancellationToken);
            
            if (pages.Count == 0)
            {
                return NotFound(
                    new ApiResponse { Message = $"Cannot find pages for InvoiceId: {invoiceId}." });
            }

            return Ok(
                new ApiResponse<List<InvoicePage>>() { Data = pages });
        }

        [HttpGet("{pageId}/imageLink")]
        public async Task<IActionResult> GetImageLinkAsync(int pageId, CancellationToken cancellationToken)
        {
            if (pageId < 1)
            {
                return BadRequest(
                    new ApiResponse { Message = "PageId should be greater then 0." });
            }

            var pageLink = await invoicePageService.GetImageLinkAsync(pageId, cancellationToken);
            
            if (pageLink == null)
            {
                return NotFound(
                    new ApiResponse { Message = $"Cannot find page with PageId: {pageId}." });
            }

            return Ok(
                new ApiResponse<string>() { Data = pageLink });
        }

        private readonly IInvoicePageService invoicePageService;
    }
}
