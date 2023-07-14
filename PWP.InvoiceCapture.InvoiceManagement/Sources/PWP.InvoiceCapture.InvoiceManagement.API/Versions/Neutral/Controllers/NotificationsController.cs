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
    [ApiController]
    [Route("notifications")]
    public class NotificationsController : ControllerBase
    {
        public NotificationsController(IWebhookService hooksService)
        {
            Guard.IsNotNull(hooksService, nameof(hooksService));

            this.hooksService = hooksService;
        }

        [HttpGet]
        [Route("webhooks")]
        public async Task<IActionResult> GetWebhooksListAsync(CancellationToken cancellationToken)
        {
            var hooks = await hooksService.GetListAsync(cancellationToken);

            return Ok(
                new ApiResponse<List<Webhook>> { Data = hooks });
        }

        [HttpGet]
        [Route("webhooks/{webhookId}")]
        public async Task<IActionResult> GetWebhookByIdAsync(int webhookId, CancellationToken cancellationToken)
        {
            if (webhookId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(webhookId)} parameter has invalid value." });
            }

            var webhook = await hooksService.GetAsync(webhookId, cancellationToken);

            if (webhook == null)
            {
                return new NotFoundObjectResult(
                    new ApiResponse { Message = $"Webhook registration request with id = {webhookId} not found." });
            }

            return Ok(
                new ApiResponse<Webhook> { Data = webhook });
        }

        [HttpPost]
        [Route("webhooks")]
        public async Task<IActionResult> CreateWebhookAsync(Webhook hook, CancellationToken cancellationToken)
        {
            var result = await hooksService.CreateAsync(hook, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut]
        [Route("webhooks/{webhookId}")]
        public async Task<IActionResult> UpdateAsync(int webhookId, Webhook hook, CancellationToken cancellationToken)
        {
            if (webhookId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(webhookId)} parameter has invalid value." });
            }

            var updateResult = await hooksService.UpdateAsync(webhookId, hook, cancellationToken);

            return updateResult.ToActionResult();
        }

        [HttpDelete]
        [Route("webhooks/{webhookId}")]
        public async Task<IActionResult> DeleteAsync(int webhookId, CancellationToken cancellationToken)
        {
            if (webhookId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(webhookId)} parameter has invalid value." });
            }

            await hooksService.DeleteAsync(webhookId, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("webhooks/triggertypes")]
        public Dictionary<string, int> GetTriggerTypes()
        {
            return hooksService.GetTriggerTypes();
        }

        private readonly IWebhookService hooksService;
    }
}
