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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.TenantCapture.Identity.API.UnitTests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TenantsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            tenantServiceMock = mockRepository.Create<ITenantService>();
            target = new TenantsController(tenantServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenTenantServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantsController(null));
        }

        [TestMethod]
        public async Task GetTenantsListAsync_WhenTenantsCollectionExists_ShouldReturnOkResultWithTenantListAsync()
        {
            var expectedTenants = new List<Tenant> { new Tenant() };

            tenantServiceMock
                .Setup(tenantService => tenantService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedTenants);

            var result = await target.GetTenantsListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<Tenant>>;

            Assert.IsInstanceOfType(apiResponse.Data, expectedTenants.GetType());
            Assert.AreEqual(expectedTenants, apiResponse.Data);
        }

        [TestMethod]
        public async Task GetTenantListAsync_WhenTenantsNotFound_ShouldReturnOkResultWithNullAsync()
        {
            tenantServiceMock
                .Setup(tenantService => tenantService.GetListAsync(cancellationToken))
                .ReturnsAsync((List<Tenant>)null);

            var result = await target.GetTenantsListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<Tenant>>;

            Assert.IsNull(apiResponse.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetTenantByIdAsync_WhenTenantFound_ShouldReturnOkResultWithTenantAsync(int tenantId)
        {
            var expectedTenant = new Tenant();

            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(expectedTenant);

            var result = await target.GetTenantByIdAsync(tenantId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<Tenant>;

            Assert.IsNotNull(apiResponse);
            Assert.IsInstanceOfType(apiResponse.Data, expectedTenant.GetType());
            Assert.AreEqual(expectedTenant, apiResponse.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetTenantByIdAsync_WhenTenantNotFound_ShouldReturn404NotFoundResultAsync(int tenantId)
        {
            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync((Tenant)null);

            var result = await target.GetTenantByIdAsync(tenantId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Tenant with id = {tenantId} not found.");
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetTenantByIdAsync_WhenTenantIdIsZeroOrNegative_ShouldReturnBadRequestResultAsync(int tenantId)
        {
            var result = await target.GetTenantByIdAsync(tenantId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "tenantId parameter has invalid value.");
        }

        [TestMethod]
        public async Task CloneTenantAsync_ShouldCallServiceAsync()
        {
            var actualIds = new List<int>();
            var actualNames = new List<string>();
            tenantServiceMock
                .Setup(tenantService => tenantService.CloneAsync(Capture.In(actualIds), Capture.In(actualNames), cancellationToken))
                .ReturnsAsync(new OperationResult<Tenant>());

            var result = await target.CloneTenantAsync(tenantId, tenantName, cancellationToken);

            Assert.AreEqual(1, actualIds.Count());
            Assert.AreEqual(1, actualNames.Count());
            Assert.AreEqual(tenantId, actualIds[0]);
            Assert.AreEqual(tenantName, actualNames[0]);

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

        private Mock<ITenantService> tenantServiceMock;
        private TenantsController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int tenantId = 123;
        private const string tenantName = "SomeName";
    }
}
