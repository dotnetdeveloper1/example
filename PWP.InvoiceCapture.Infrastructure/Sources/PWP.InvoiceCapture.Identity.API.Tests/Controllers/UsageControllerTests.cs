using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.API.Controllers;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.TenantCapture.Identity.API.UnitTests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UsageControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            usageServiceMock = mockRepository.Create<IUsageService>();
            target = new UsageController(usageServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenUsageServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UsageController(null));
        }

        [TestMethod]
        public async Task GetUsageAsync_ShouldReturnOkResultWithUsageAsync()
        {

            usageServiceMock
                .Setup(usageService => usageService.GetUsageAsync(groupId, cancellationToken))
                .ReturnsAsync(new Usage());

            var result = await target.GetUsageAsync(groupId, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<Usage>;

            Assert.IsInstanceOfType(apiResponse.Data, typeof(Usage));
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
        }

        private Mock<IUsageService> usageServiceMock;
        private UsageController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int groupId = 15;
    }
}
