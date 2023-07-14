using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;

namespace PWP.InvoiceCapture.Identity.API.Controllers
{
    [Route("groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        public GroupsController(IGroupService groupService)
        {
            Guard.IsNotNull(groupService, nameof(groupService));

            this.groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupsListAsync(CancellationToken cancellationToken)
        {
            var groups = await groupService.GetListAsync(cancellationToken);

            return Ok(
                new ApiResponse<List<Group>> { Data = groups });
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupAsync(GroupCreationParameters groupCreationParameters, CancellationToken cancellationToken)
        {
            var operationResult = await groupService.CreateAsync(groupCreationParameters, cancellationToken);

            return operationResult.ToActionResult();
        }

        private readonly IGroupService groupService;
    }
}
