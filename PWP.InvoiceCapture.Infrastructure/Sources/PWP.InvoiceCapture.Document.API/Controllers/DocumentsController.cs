using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Document.Business.Contract.Services;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Document.API.Client.Models;
using PWP.InvoiceCapture.Core.Models;

namespace PWP.InvoiceCapture.Document.API.Controllers
{
    [ApiController]
    [Route("documents")]
    public class DocumentsController : ControllerBase
    {
        public DocumentsController(IDocumentService documentService)
        {
            this.documentService = documentService;
        }

        [HttpGet]
        [Route("{fileId}")]
        public async Task<IActionResult> GetStreamAsync(string fileId, CancellationToken cancellationToken)
        {
            var operationResult = await documentService.GetDocumentStreamAsync(fileId, cancellationToken);

            if (!operationResult.IsSuccessful)
            {
                return operationResult.ToActionResult();
            }

            var documentStream = operationResult.Data;
            Response.ContentLength = documentStream.Length;
            
            return new FileStreamResult(documentStream.FileStream, documentStream.ContentType) 
            {
                FileDownloadName = fileId 
            };
        }
       
        [HttpGet]
        [Route("{fileId}/link")]
        public async Task<IActionResult> GetTemporaryLinkAsync(string fileId, CancellationToken cancellationToken)
        {
            var link = await documentService.GetTemporaryLinkAsync(fileId, cancellationToken);
            var response = new ApiResponse<string> 
            { 
                Data = link.Data,
                Code = link.Code, 
                Message = link.Message 
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFileAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                var response = new ApiResponse { Message = "File is null." };

                return new BadRequestObjectResult(response);
            }

            var extension = Path.GetExtension(file.FileName);

            using (var fileStream = file.OpenReadStream())
            {
                var fileId = await documentService.CreateDocumentAsync(fileStream, extension, cancellationToken);
                var data = new UploadDocumentResponse { FileId = fileId };
                var response = new ApiResponse<UploadDocumentResponse> { Data = data, Message = "File was uploaded successfully." };

                return Ok(response);
            }
        }

        private readonly IDocumentService documentService;
    }
}
