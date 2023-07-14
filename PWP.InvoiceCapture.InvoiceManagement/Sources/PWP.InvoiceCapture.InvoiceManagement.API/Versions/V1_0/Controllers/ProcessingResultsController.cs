using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.API.Versions.V1_0.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("processingresults")]
    public class ProcessingResultsController : ControllerBase
    {
        public ProcessingResultsController(IInvoiceProcessingResultService invoiceProcessingResultService)
        {
            Guard.IsNotNull(invoiceProcessingResultService, nameof(invoiceProcessingResultService));

            this.invoiceProcessingResultService = invoiceProcessingResultService;
        }

        [HttpGet]
        [Route("{processingResultId}")]
        public async Task<IActionResult> GetProcessingResultByIdAsync(int processingResultId, CancellationToken cancellationToken)
        {
            if (processingResultId < 1)
            {
                return BadRequest(
                    new ApiResponse { Message = "ProcessingResultsId should be greater then 0." });
            }

            var processingResult = await invoiceProcessingResultService.GetAsync(processingResultId, cancellationToken);

            if (processingResult == null)
            {
                return NotFound(
                    new ApiResponse { Message = $"Cannot find processing result for ProcessingResultId: {processingResultId}." });
            }

            return Ok(
                new ApiResponse<InvoiceProcessingResult> { Data = processingResult });
        }

        [HttpGet]
        [Route("invoices/{invoiceId}")]
        public async Task<IActionResult> GetProcessingResultsByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            if (invoiceId < 1)
            {
                return BadRequest(
                    new ApiResponse { Message = "InvoiceId should be greater then 0." });
            }

            var processingResults = await invoiceProcessingResultService.GetListAsync(invoiceId, cancellationToken);

            if (processingResults.Count == 0)
            {
                return NotFound(
                    new ApiResponse { Message = $"Cannot find processing results for InvoiceId: {invoiceId}." });
            }

            return Ok(
                new ApiResponse<List<InvoiceProcessingResult>> { Data = processingResults });
        }

        [HttpGet]
        [Route("invoices/{invoiceId}/latest")]
        public async Task<IActionResult> GetLatestProcessingResultByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            if (invoiceId < 1)
            {
                return BadRequest(
                    new ApiResponse { Message = "InvoiceId should be greater then 0." });
            }

            var processingResult = await invoiceProcessingResultService.GetLatestAsync(invoiceId, cancellationToken);

            if (processingResult == null)
            {
                return NotFound(
                    new ApiResponse { Message = $"Cannot find processing result for InvoiceId: {invoiceId}." });
            }

            return Ok(
                new ApiResponse<InvoiceProcessingResult> { Data = processingResult });
        }


        [HttpPut]
        [Route("{processingResultId}/dataAnnotation")]
        public async Task<IActionResult> UpdateDataAnnotationAsync(int processingResultId, UpdatedDataAnnotation updatedDataAnnotation, CancellationToken cancellationToken)
        {
            if (processingResultId < 1)
            {
                return BadRequest(
                    new ApiResponse { Message = "ProcessingResultId should be greater then 0." });
            }

            var operationResult = await invoiceProcessingResultService.UpdateDataAnnotationAsync(processingResultId, updatedDataAnnotation, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpPut]
        [Route("{processingResultId}/complete")]
        public async Task<IActionResult> CompleteAsync(int processingResultId, UpdatedDataAnnotation updatedDataAnnotation, CancellationToken cancellationToken)
        {
            if (processingResultId < 1)
            {
                return BadRequest(
                    new ApiResponse { Message = "ProcessingResultId should be greater then 0." });
            }

            var operationResult = await invoiceProcessingResultService.CompleteAsync(processingResultId, updatedDataAnnotation, cancellationToken);

            return operationResult.ToActionResult();
        }

        private readonly IInvoiceProcessingResultService invoiceProcessingResultService;
    }
}
