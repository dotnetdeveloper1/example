using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Mappers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Invoicev0_9 = PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Models.Invoice;

namespace PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Controllers
{
    [ApiController]
    [Route("invoices")]
    [ApiVersion("0.9")]
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
            var mappedInvoices = InvoiceMapper.ToV0_9Models(invoices);

            return Ok(
                new ApiResponse<List<Invoicev0_9>> { Data = mappedInvoices });
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetActiveInvoiceListAsync(CancellationToken cancellationToken)
        {
            var invoices = await invoiceService.GetActiveListAsync(cancellationToken);
            var mappedInvoices = InvoiceMapper.ToV0_9Models(invoices);

            return Ok(
                new ApiResponse<List<Invoicev0_9>> { Data = mappedInvoices });
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
            var mappedInvoice = InvoiceMapper.ToV0_9Model(invoice);

            return Ok(
                new ApiResponse<Invoicev0_9> { Data = mappedInvoice });
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

            var mappedInvoice = InvoiceMapper.ToV0_9Model(invoice);

            return Ok(
                new ApiResponse<Invoicev0_9> { Data = mappedInvoice });
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

        private readonly IInvoiceService invoiceService;
    }
}
