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
    public class CulturesControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            cultureServiceMock = mockRepository.Create<ICultureService>();
            target = new CulturesController(cultureServiceMock.Object);
        }

        [TestMethod]
        public async Task GetListAsync_WhenGroupsCollectionExists_ShouldReturnOkResultWithCulturesListAsync()
        {
            var expectedCultures = new List<Culture> { new Culture() };

            cultureServiceMock
                .Setup(culturesService => culturesService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedCultures);

            var result = await target.GetCulturesListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<Culture>>;

            Assert.IsInstanceOfType(apiResponse.Data, expectedCultures.GetType());
            Assert.AreEqual(expectedCultures, apiResponse.Data);
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
        }

        private Mock<ICultureService> cultureServiceMock;
        private CulturesController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
