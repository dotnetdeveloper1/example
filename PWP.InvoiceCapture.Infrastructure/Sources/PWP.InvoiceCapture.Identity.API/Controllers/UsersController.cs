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
    [Route("users")]
    public class UsersController : ControllerBase
    {
        public UsersController(IUserService userService)
        {
            Guard.IsNotNull(userService, nameof(userService));

            this.userService = userService;
        }

        [HttpGet]
        [Route("groups/{groupId}")]
        public async Task<IActionResult> GetUsersListAsync(int groupId, CancellationToken cancellationToken)
        {
            if (groupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(groupId)} parameter has invalid value." });
            }

            var users = await userService.GetListAsync(groupId, cancellationToken);

            return Ok(
                new ApiResponse<List<User>> { Data = users });
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
        {
            if (userId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(userId)} parameter has invalid value." });
            }

            var user = await userService.GetAsync(userId, cancellationToken);

            if (user == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"User with id {userId} not found." });
            }

            return Ok(new ApiResponse<User> { Data = user });
        }

        [HttpPost]
        [Route("groups/{groupId}")]
        public async Task<IActionResult> CreateUserAsync(int groupId, CancellationToken cancellationToken)
        {
            if (groupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(groupId)} parameter has invalid value." });
            }

            var operationResult = await userService.CreateAsync(groupId, cancellationToken);

            return operationResult.ToActionResult();
        }

        private readonly IUserService userService;
    }
}
