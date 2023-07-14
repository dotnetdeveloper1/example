using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.API.Controllers;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.API.UnitTests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TenantsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            tenantSettingServiceMock = mockRepository.Create<ITenantSettingService>();
            target = new TenantSettingsController(tenantSettingServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenTenantSettingServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantSettingsController(null));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetByTenantIdAsync_WhenTenantSettingFound_ShouldReturnOkResultWithTenantAsync(int tenantId)
        {
            var expectedTenantSetting = new TenantSetting();

            tenantSettingServiceMock
                .Setup(tenantSettingService => tenantSettingService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(expectedTenantSetting);

            var result = await target.GetByTenantIdAsync(tenantId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<TenantSetting>;

            Assert.IsNotNull(apiResponse);
            Assert.IsInstanceOfType(apiResponse.Data, expectedTenantSetting.GetType());
            Assert.AreEqual(expectedTenantSetting, apiResponse.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetByTenantIdAsync_WhenTenantNotFound_ShouldReturn404NotFoundResultAsync(int tenantId)
        {
            tenantSettingServiceMock
                .Setup(tenantSettingService => tenantSettingService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync((TenantSetting)null);

            var result = await target.GetByTenantIdAsync(tenantId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"TenantSetting with id = {tenantId} not found.");
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetByTenantIdAsync_WhenTenantIdIsZeroOrNegative_ShouldReturnBadRequestResultAsync(int tenantId)
        {
            var result = await target.GetByTenantIdAsync(tenantId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "tenantId parameter has invalid value.");
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

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
        }

        private Mock<ITenantSettingService> tenantSettingServiceMock;
        private TenantSettingsController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
