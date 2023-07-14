using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.API.Controllers;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.API.UnitTests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PlansControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            planServiceMock = mockRepository.Create<IPlanService>();
            target = new PlansController(planServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenPlanServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PlansController(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetGroupPlansListAsync_WhenGroupIdIsNotValid_ShouldReturnBadRequestAsync(int groupId)
        {
            var result = await target.GetGroupPlansListAsync(groupId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "groupId is an invalid value. Should be greater than zero.");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task CreateGroupPlanAsync_WhenGroupIdIsNotValid_ShouldReturnBadRequestAsync(int groupId)
        {
            var result = await target.CreateGroupPlanAsync(groupId, 1, DateTime.Now, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "groupId is an invalid value. Should be greater than zero.");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task CreateGroupPlanAsync_WhenPlanIdIsNotValid_ShouldReturnBadRequestAsync(int planId)
        {
            var result = await target.CreateGroupPlanAsync(1, planId, DateTime.Now, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "planId is an invalid value. Should be greater than zero.");
        }


        [TestMethod]
        public async Task GetGroupPlansListAsync_WhenPlansCollectionExists_ShouldReturnOkResultWithPlansListAsync()
        {
            var expectedPlans = new List<GroupPlan> { new GroupPlan() };

            planServiceMock
                .Setup(planService => planService.GetGroupPlanListAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedPlans);

            var result = await target.GetGroupPlansListAsync(groupId, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<GroupPlan>>;

            Assert.IsInstanceOfType(apiResponse.Data, expectedPlans.GetType());
            Assert.AreEqual(expectedPlans, apiResponse.Data);
        }

        [TestMethod]
        public async Task CreateGroupPlanAsync_WhenParamsAreCorrect_ShouldCreateGroupPlanAsync()
        {
            planServiceMock
                .Setup(planService => planService.CreateGroupPlanAsync(groupId, planId, date, cancellationToken))
                .ReturnsAsync(OperationResult.Success);

            var result = await target.CreateGroupPlanAsync(groupId, planId, date, cancellationToken) as OkObjectResult;
        }

        [TestMethod]
        public async Task CreatePlanAsync_WhenParamsAreCorrect_ShouldCreatePlanAsync()
        {
            var planCreationParameters = new PlanCreationParameters()
            {
                PlanName = planName,
                TypeId = planType,
                AllowedDocumentsCount = allowedDocumentsCount,
                CurrencyId = currencyId,
                Price = price
            };

            var actualCreationParams = new List<PlanCreationParameters>();

            planServiceMock
                .Setup(planService => planService.CreatePlanAsync(Capture.In(actualCreationParams), cancellationToken))
                .ReturnsAsync(new OperationResult<int>());

            var result = await target.CreatePlanAsync(planCreationParameters, cancellationToken) as OkObjectResult;

            Assert.AreEqual(1, actualCreationParams.Count);
            var actual = actualCreationParams[0];
            Assert.AreEqual(planCreationParameters.PlanName, actual.PlanName);
            Assert.AreEqual(planCreationParameters.TypeId, actual.TypeId);
            Assert.AreEqual(planCreationParameters.AllowedDocumentsCount, actual.AllowedDocumentsCount);
            Assert.AreEqual(planCreationParameters.CurrencyId, actual.CurrencyId);
            Assert.AreEqual(planCreationParameters.Price, actual.Price);
        }

        [TestMethod]
        public async Task GetPlansListAsync_WhenPlansCollectionExists_ShouldReturnOkResultWithPlansListAsync()
        {
            var expectedPlans = new List<Plan> { new Plan() };

            planServiceMock
                .Setup(planServiceMock => planServiceMock.GetPlanListAsync(cancellationToken))
                .ReturnsAsync(expectedPlans);

            var result = await target.GetPlansListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<Plan>>;

            Assert.IsInstanceOfType(apiResponse.Data, expectedPlans.GetType());
            Assert.AreEqual(expectedPlans, apiResponse.Data);
        }
        
        [DataRow(1)]
        [DataRow(15)]
        [TestMethod]
        public async Task DeleteActivePlanAsync_ShouldCallRepositoryAndReturnActionResultAsync(int groupId)
        {
            planServiceMock
                .Setup(planServiceMock => planServiceMock.DeleteActiveAsync(groupId, cancellationToken))
                .ReturnsAsync(new OperationResult());

            var result = await target.DeleteActivePlanAsync(groupId, cancellationToken) as ActionResult;

            Assert.IsNotNull(result);
        }

        [DataRow(2)]
        [DataRow(16)]
        [TestMethod]
        public async Task DeleteGroupPlanAsync_ShouldCallRepositoryAndReturnActionResultAsync(int groupPlanId)
        {
            planServiceMock
                .Setup(planServiceMock => planServiceMock.DeleteGroupPlanByIdAsync(groupPlanId, cancellationToken))
                .ReturnsAsync(new OperationResult());

            var result = await target.DeleteGroupPlanAsync(groupPlanId, cancellationToken) as ActionResult;

            Assert.IsNotNull(result);
        }

        [DataRow(1)]
        [DataRow(15)]
        [TestMethod]
        public async Task CancelRenewalAsync_ShouldCallRepositoryAndReturnActionResultAsync(int groupId)
        {
            planServiceMock
                .Setup(planServiceMock => planServiceMock.CancelRenewalAsync(groupId, cancellationToken))
                .ReturnsAsync(new OperationResult());

            var result = await target.CancelRenewalAsync(groupId, cancellationToken) as ActionResult;

            Assert.IsNotNull(result);
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType, string expectedMessage)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;

            Assert.IsNotNull(objectResult.Value);
            Assert.IsInstanceOfType(objectResult.Value, typeof(ApiResponse));

            var apiResponse = (ApiResponse)objectResult.Value;
            Assert.AreEqual(expectedMessage, apiResponse.Message);
        }

        private Mock<IPlanService> planServiceMock;
        private PlansController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int groupId = 1;
        private const int planId = 11;
        private readonly DateTime date = DateTime.Parse("2020-01-02", CultureInfo.InvariantCulture);
        private const string planName = "somePlanName";
        private const int allowedDocumentsCount = 145;
        private const decimal price = 12.5m;
        private const int currencyId = 1;
        private const int planType = 2;
    }
}
