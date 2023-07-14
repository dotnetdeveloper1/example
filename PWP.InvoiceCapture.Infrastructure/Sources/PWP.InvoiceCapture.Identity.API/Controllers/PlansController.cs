using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.API.Controllers
{
    [ApiController]
    [Route("plans")]
    public class PlansController: ControllerBase
    {
        public PlansController(IPlanService planService)
        {
            Guard.IsNotNull(planService, nameof(planService));

            this.planService = planService;
        }

        [HttpGet]
        [Route("groups/{groupId}")]
        public async Task<IActionResult> GetGroupPlansListAsync(int groupId, CancellationToken cancellationToken)
        {
            if (groupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(groupId)} is an invalid value. Should be greater than zero." });
            }

            var groupPlans = await planService.GetGroupPlanListAsync(groupId, cancellationToken);

            return Ok(
                new ApiResponse<List<GroupPlan>> { Data = groupPlans });
        }

        [HttpPost]
        [Route("groups/{groupId}")]
        public async Task<IActionResult> CreateGroupPlanAsync(int groupId, int planId, DateTime startDateUtc, CancellationToken cancellationToken)
        {
            if (groupId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(groupId)} is an invalid value. Should be greater than zero." });
            }
            if (planId <= 0)
            {
                return new BadRequestObjectResult(
                    new ApiResponse { Message = $"{nameof(planId)} is an invalid value. Should be greater than zero." });
            }

            var operationResult = await planService.CreateGroupPlanAsync(groupId, planId, startDateUtc, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpDelete]
        [Route("groups/{groupId}/active")]
        public async Task<IActionResult> DeleteActivePlanAsync(int groupId, CancellationToken cancellationToken)
        {
            var operationResult = await planService.DeleteActiveAsync(groupId, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpDelete]
        [Route("groupPlans/{groupPlanId}")]
        public async Task<IActionResult> DeleteGroupPlanAsync(int groupPlanId, CancellationToken cancellationToken)
        {
            var operationResult = await planService.DeleteGroupPlanByIdAsync(groupPlanId, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetPlansListAsync(CancellationToken cancellationToken)
        {

            var plans = await planService.GetPlanListAsync(cancellationToken);

            return Ok(
                new ApiResponse<List<Plan>> { Data = plans });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlanAsync(PlanCreationParameters planCreationParameters, CancellationToken cancellationToken)
        {
            var operationResult = await planService.CreatePlanAsync(planCreationParameters, cancellationToken);

            return operationResult.ToActionResult();
        }

        [HttpPost]
        [Route("groups/{groupId}/cancelRenewal")]
        public async Task<IActionResult> CancelRenewalAsync(int groupId, CancellationToken cancellationToken)
        {
            var operationResult = await planService.CancelRenewalAsync(groupId, cancellationToken);

            return operationResult.ToActionResult();
        }


        private readonly IPlanService planService;
    }
}
