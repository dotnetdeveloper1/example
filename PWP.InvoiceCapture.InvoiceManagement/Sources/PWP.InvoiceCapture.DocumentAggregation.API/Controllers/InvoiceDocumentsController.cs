using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.DocumentAggregation.API.Constants;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Models;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.API.Controllers
{
    [ApiController]
    [Route("invoiceDocuments")]
    public class InvoiceDocumentsController : ControllerBase
    {
        public InvoiceDocumentsController(IInvoiceDocumentService invoiceDocumentService) 
        {
            Guard.IsNotNull(invoiceDocumentService, nameof(invoiceDocumentService));

            this.invoiceDocumentService = invoiceDocumentService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocumentAsync(IFormFile file, CancellationToken cancellationToken)
        {
            var fileValidationMessage = GetFileValidationMessage(file);
            if(!string.IsNullOrWhiteSpace(fileValidationMessage))
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = fileValidationMessage });
            }

            using (var fileStream = file.OpenReadStream())
            {
                var uploadedDocument = await invoiceDocumentService.SaveAsync(fileStream, file.FileName, FileSourceType.API, cancellationToken);
                
                var response = new ApiResponse<UploadedDocument>
                { 
                    Message = $"{file.FileName} successfully uploaded.",
                    Data = uploadedDocument
                };

                return Ok(response);
            }
        }

        private string GetFileValidationMessage(IFormFile file)
        {
            if (file == null)
            {
                return "File is null.";
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                return "FileName is null or empty.";
            }

            if (!FileExtensions.AllowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                return "File format is not supported.";
            }

            return string.Empty;
        }

        private readonly IInvoiceDocumentService invoiceDocumentService;
    }
}
