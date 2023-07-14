using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.API.Controllers
{
    [ApiController]
    [Route("packs")]
    public class PacksController: ControllerBase
    {
        public PacksController(IPackService packService)
        {
            Guard.IsNotNull(packService, nameof(packService));

            this.packService = packService;
        }

        [HttpGet]
        [Route("groups/{groupId}")]
        public async Task<IActionResult> GetGroupPacksListAsync(int groupId, CancellationToken cancellationToken)
        {
            if (groupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(groupId)} is an invalid value. Should be greater than zero." });
            }

            var groupPack = await packService.GetGroupPackListAsync(groupId, cancellationToken);

            return Ok(
                new ApiResponse<List<GroupPack>> { Data = groupPack });
        }

        [HttpPost]
        [Route("groups/{groupId}")]
        public async Task<IActionResult> CreateGroupPackAsync(int groupId, int packId, CancellationToken cancellationToken)
        {
            if (groupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(groupId)} is an invalid value. Should be greater than zero." });
            }
            if (packId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(packId)} is an invalid value. Should be greater than zero." });
            }

            var operationResult = await packService.CreateGroupPackAsync(groupId, packId, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetPacksListAsync(CancellationToken cancellationToken)
        {

            var packs = await packService.GetPackListAsync(cancellationToken);

            return Ok(
                new ApiResponse<List<Pack>> { Data = packs });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackAsync(PackCreationParameters packCreationParameters, CancellationToken cancellationToken) 
        {
            var operationResult = await packService.CreatePackAsync(packCreationParameters, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpDelete]
        [Route("groupPacks/{groupPackId}")]
        public async Task<IActionResult> DeleteGroupPackAsync(int groupPackId, CancellationToken cancellationToken)
        {
            var operationResult = await packService.DeleteGroupPackByIdAsync(groupPackId, cancellationToken);

            return operationResult.ToActionResult();
        }

        private readonly IPackService packService;
    }
}
