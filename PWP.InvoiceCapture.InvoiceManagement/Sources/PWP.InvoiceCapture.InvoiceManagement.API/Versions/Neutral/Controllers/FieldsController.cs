using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.API.Mappers;
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
    [Route("fields")]
    public class FieldsController : ControllerBase
    {
        public FieldsController(IFieldService fieldService)
        {
            Guard.IsNotNull(fieldService, nameof(fieldService));

            this.fieldService = fieldService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFieldsListAsync(CancellationToken cancellationToken)
        {
            var fields = await fieldService.GetListAsync(cancellationToken);

            return Ok(
                new ApiResponse<List<Field>> { Data = fields });
        }

        [HttpGet]
        [Route("{fieldId}")]
        public async Task<IActionResult> GetFieldByIdAsync(int fieldId, CancellationToken cancellationToken)
        {
            if (fieldId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(fieldId)} parameter has invalid value." });
            }

            var field = await fieldService.GetAsync(fieldId, cancellationToken);

            if (field == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"Field with id = {fieldId} not found." });
            }

            return Ok(
                new ApiResponse<Field> { Data = field });
        }

        [HttpPost]
        public async Task<IActionResult> CreateFieldAsync(Field field, CancellationToken cancellationToken)
        {
            var result = await fieldService.CreateAsync(field, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut]
        [Route("{fieldId}")]
        public async Task<IActionResult> UpdateAsync(int fieldId, Field field, CancellationToken cancellationToken)
        {
            if (fieldId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(fieldId)} parameter has invalid value." });
            }

            var updateResult = await fieldService.UpdateAsync(fieldId, field, cancellationToken);

            return updateResult.ToActionResult();
        }

        [HttpDelete]
        [Route("{fieldId}")]
        public async Task<IActionResult> DeleteAsync(int fieldId, CancellationToken cancellationToken)
        {
            if (fieldId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(fieldId)} parameter has invalid value." });
            }

            var deleteResult = await fieldService.DeleteAsync(fieldId, cancellationToken);

            return deleteResult.ToActionResult();
        }

        [HttpGet]
        [Route("types")]
        public Dictionary<string, int> GetFieldTypes()
        {
            return fieldService.GetFieldTypes();
        }

        [HttpGet]
        [Route("targetfieldtypes")]
        public Dictionary<string, int> GetTargetFieldTypes()
        {
            return fieldService.GetTargetFieldTypes();
        }

        private readonly IFieldService fieldService;
    }
}
