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
    [Route("fieldgroups")]
    public class FieldGroupsController : ControllerBase
    {
        public FieldGroupsController(IFieldGroupService fieldGroupService)
        {
            Guard.IsNotNull(fieldGroupService, nameof(fieldGroupService));

            this.fieldGroupService = fieldGroupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFieldGroupsListAsync(CancellationToken cancellationToken)
        {
            var fieldGroups = await fieldGroupService.GetListAsync(cancellationToken);

            return Ok(
                new ApiResponse<List<FieldGroup>> { Data = fieldGroups });
        }


        [HttpGet]
        [Route("{fieldGroupId}")]
        public async Task<IActionResult> GetFieldGroupByIdAsync(int fieldGroupId, CancellationToken cancellationToken)
        {
            if (fieldGroupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(fieldGroupId)} parameter has invalid value." });
            }

            var fieldGroup = await fieldGroupService.GetAsync(fieldGroupId, cancellationToken);

            if (fieldGroup == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"FieldGroup with id = {fieldGroupId} not found." });
            }

            return Ok(
                new ApiResponse<FieldGroup> { Data = fieldGroup });
        }

        [HttpPost]
        public async Task<IActionResult> CreateFieldGroupAsync(FieldGroup fieldGroup, CancellationToken cancellationToken)
        {
            await fieldGroupService.CreateAsync(fieldGroup, cancellationToken);

            return Ok(
                new ApiResponse<FieldGroup> { Data = fieldGroup });
        }

        [HttpPut]
        [Route("{fieldGroupId}")]
        public async Task<IActionResult> UpdateFieldGroupAsync(int fieldGroupId, FieldGroup fieldGroup, CancellationToken cancellationToken)
        {
            if (fieldGroupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(fieldGroupId)} parameter has invalid value." });
            }

            var updateResult = await fieldGroupService.UpdateAsync(fieldGroupId, fieldGroup, cancellationToken);

            return updateResult.ToActionResult();
        }

        [HttpDelete]
        [Route("{fieldGroupId}")]
        public async Task<IActionResult> DeleteFieldGroupAsync(int fieldGroupId, CancellationToken cancellationToken)
        {
            if (fieldGroupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(fieldGroupId)} parameter has invalid value." });
            }

            var deleteResult = await fieldGroupService.DeleteAsync(fieldGroupId, cancellationToken);

            return deleteResult.ToActionResult();
        }

        private readonly IFieldGroupService fieldGroupService;
    }
}
