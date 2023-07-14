using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.API.Controllers
{
    [ApiController]
    [Route("invoicetemplates")]
    public class InvoiceTemplatesController : ControllerBase
    {
        public InvoiceTemplatesController(ITemplateManagementService templateManagementService) 
        {
            Guard.IsNotNull(templateManagementService, nameof(templateManagementService));

            this.templateManagementService = templateManagementService;
        }

        [HttpGet]
        [Route("{templateId}/trainingscount")]
        public async Task<IActionResult> GetTrainingsCountAsync(int templateId, CancellationToken cancellationToken)
        {
            if (templateId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(templateId)} parameter has invalid value." });
            }

            var template = await templateManagementService.GetTemplateAsync(templateId, cancellationToken);

            if (template == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"Template with id = {templateId} not found." });
            }

            var count = await templateManagementService.GetTemplateTrainingsCountAsync(template.Id, cancellationToken);

            return Ok(
                new ApiResponse<int> { Data = count });
        }

        private readonly ITemplateManagementService templateManagementService;
    }
}
