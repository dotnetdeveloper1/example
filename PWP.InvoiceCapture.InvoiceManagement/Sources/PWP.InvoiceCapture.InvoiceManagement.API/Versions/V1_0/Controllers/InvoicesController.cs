using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.API.Versions.V1_0.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("invoices")]
    public class InvoicesController : ControllerBase
    {
        public InvoicesController(IInvoiceService invoiceService)
        {
            Guard.IsNotNull(invoiceService, nameof(invoiceService));

            this.invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceListAsync(CancellationToken cancellationToken)
        {
            var invoices = await invoiceService.GetListAsync(cancellationToken);

            return Ok(
                new ApiResponse<List<Invoice>> { Data = invoices });
        }

        [HttpGet]
        [Route("paginated")]
        public async Task<IActionResult> GetPaginatedListAsync([FromQuery] InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paginatedResult = await invoiceService.GetPaginatedListAsync(paginatedRequest, cancellationToken);
            var apiResponse = new ApiResponse<PaginatedResult<Invoice>> { Data = paginatedResult };

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetActiveInvoiceListAsync(CancellationToken cancellationToken)
        {
            var invoices = await invoiceService.GetActiveListAsync(cancellationToken);

            return Ok(
                new ApiResponse<List<Invoice>> { Data = invoices });
        }

        [HttpGet]
        [Route("active/paginated")]
        public async Task<IActionResult> GetActivePaginatedListAsync([FromQuery] InvoicePaginatedRequest paginatedRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paginatedResult = await invoiceService.GetActivePaginatedListAsync(paginatedRequest, cancellationToken);
            var apiResponse = new ApiResponse<PaginatedResult<Invoice>> { Data = paginatedResult };

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("{invoiceId}")]
        public async Task<IActionResult> GetInvoiceByIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            if (invoiceId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(invoiceId)} parameter has invalid value." });
            }

            var invoice = await invoiceService.GetAsync(invoiceId, cancellationToken);

            if (invoice == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"Invoice with id = {invoiceId} not found." });
            }

            return Ok(
                new ApiResponse<Invoice> { Data = invoice });
        }

        [HttpGet]
        [Route("documents/{documentId}")]
        public async Task<IActionResult> GetInvoiceByDocumentIdAsync(string documentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(documentId)} parameter is empty." });
            }

            var invoice = await invoiceService.GetAsync(documentId, cancellationToken);

            if (invoice == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"Invoice with file id = {documentId} not found." });
            }

            return Ok(
                new ApiResponse<Invoice> { Data = invoice });
        }

        [HttpGet]
        [Route("{invoiceId}/document")]
        public async Task<IActionResult> GetDocumentFileLinkAsync(int invoiceId, CancellationToken cancellationToken)
        {
            if (invoiceId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(invoiceId)} parameter has invalid value." });
            }

            var documentLink = await invoiceService.GetDocumentFileLinkAsync(invoiceId, cancellationToken);

            if (documentLink == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"Document file for Invoice with id = {invoiceId} not found." });
            }

            return Ok(
                new ApiResponse<string> { Data = documentLink });
        }

        [HttpPut]
        [Route("{invoiceId}/archive")]
        public async Task<IActionResult> ArchiveInvoiceAsync(int invoiceId, CancellationToken cancellationToken)
        {
            if (invoiceId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(invoiceId)} parameter has invalid value." });
            }

            var operationResult = await invoiceService.UpdateStateAsync(invoiceId, InvoiceState.Archived, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpPut]
        [Route("{invoiceId}/redo")]
        public async Task<IActionResult> RedoInvoiceAsync(int invoiceId, CancellationToken cancellationToken)
        {
            if (invoiceId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(invoiceId)} parameter has invalid value." });
            }

            var operationResult = await invoiceService.RedoAsync(invoiceId, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpGet]
        [Route("states")]
        public Dictionary<string, int> GetInvoiceStates()
        {
            return invoiceService.GetInvoiceStates();
        }

        [HttpGet]
        [Route("statuses")]
        public Dictionary<string, int> GetInvoiceStatuses()
        {
            return invoiceService.GetInvoiceStatuses();
        }

        [HttpGet]
        [Route("processingtypes")]
        public Dictionary<string, int> GetInvoiceProcessingTypes()
        {
            return invoiceService.GetInvoiceProcessingTypes();
        }

        [HttpGet]
        [Route("filesourcetypes")]
        public Dictionary<string, int> GetInvoiceFileSourceTypes()
        {
            return invoiceService.GetInvoiceFileSourceTypes();
        }

        [HttpGet]
        [Route("sortfields")]
        public Dictionary<string, int> GetInvoiceSortFields()
        {
            return invoiceService.GetInvoiceSortFields();
        }

        [HttpGet]
        [Route("sorttypes")]
        public Dictionary<string, int> GetSortTypes()
        {
            return invoiceService.GetSortTypes();
        }

        private readonly IInvoiceService invoiceService;
    }
}
