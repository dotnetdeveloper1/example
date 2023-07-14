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
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.API.UnitTests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PacksControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            packServiceMock = mockRepository.Create<IPackService>();
            target = new PacksController(packServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenPackServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PacksController(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetGroupPacksListAsync_WhenGroupIdIsNotValid_ShouldReturnBadRequestAsync(int groupId)
        {
            var result = await target.GetGroupPacksListAsync(groupId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "groupId is an invalid value. Should be greater than zero.");
        }
       
        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task CreateGroupPackAsync_WhenGroupIdIsNotValid_ShouldReturnBadRequestAsync(int groupId)
        {
            var result = await target.CreateGroupPackAsync(groupId, 1, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "groupId is an invalid value. Should be greater than zero.");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task CreateGroupPackAsync_WhenPackIdIsNotValid_ShouldReturnBadRequestAsync(int packId)
        {
            var result = await target.CreateGroupPackAsync(1, packId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "packId is an invalid value. Should be greater than zero.");
        }


        [TestMethod]
        public async Task GetGroupPacksListAsync_WhenPacksCollectionExists_ShouldReturnOkResultWithPacksListAsync()
        {
            var expectedPacks  = new List<GroupPack> { new GroupPack() };

            packServiceMock
                .Setup(packService => packService.GetGroupPackListAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedPacks);

            var result = await target.GetGroupPacksListAsync(groupId, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<GroupPack>>;

            Assert.IsInstanceOfType(apiResponse.Data, expectedPacks.GetType());
            Assert.AreEqual(expectedPacks, apiResponse.Data);
        }

        [TestMethod]
        public async Task CreatePackAsync_WhenParamsAreCorrect_ShouldCreatePackAsync()
        {
            var packsCreationParameters = new PackCreationParameters()
            {
                PackName = packName,
                AllowedDocumentsCount = allowedDocumentsCount,
                CurrencyId = currencyId,
                Price = price
            };

            var actualCreationParams = new List<PackCreationParameters>();

            packServiceMock
                .Setup(packService => packService.CreatePackAsync(Capture.In(actualCreationParams), cancellationToken))
                .ReturnsAsync(new OperationResult<int>());

            var result = await target.CreatePackAsync(packsCreationParameters, cancellationToken) as OkObjectResult;
            
            Assert.AreEqual(1, actualCreationParams.Count);
            var actual = actualCreationParams[0];
            Assert.AreEqual(packsCreationParameters.PackName, actual.PackName);
            Assert.AreEqual(packsCreationParameters.AllowedDocumentsCount, actual.AllowedDocumentsCount);
            Assert.AreEqual(packsCreationParameters.CurrencyId, actual.CurrencyId);
            Assert.AreEqual(packsCreationParameters.Price, actual.Price);
        }

        [TestMethod]
        public async Task GetPacksListAsync_WhenPacksCollectionExists_ShouldReturnOkResultWithPacksListAsync()
        {
            var expectedPacks = new List<Pack> { new Pack() };

            packServiceMock
                .Setup(packService => packService.GetPackListAsync(cancellationToken))
                .ReturnsAsync(expectedPacks);

            var result = await target.GetPacksListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<Pack>>;

            Assert.IsInstanceOfType(apiResponse.Data, expectedPacks.GetType());
            Assert.AreEqual(expectedPacks, apiResponse.Data);
        }

        [DataRow(2)]
        [DataRow(16)]
        [TestMethod]
        public async Task DeleteGroupPackAsync_ShouldCallRepositoryAndReturnActionResultAsync(int groupPackId)
        {
            packServiceMock
                .Setup(packServiceMock => packServiceMock.DeleteGroupPackByIdAsync(groupPackId, cancellationToken))
                .ReturnsAsync(new OperationResult());

            var result = await target.DeleteGroupPackAsync(groupPackId, cancellationToken) as ActionResult;

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

        private Mock<IPackService> packServiceMock;
        private PacksController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int groupId = 1;
        private const string packName = "somePackName";
        private const int allowedDocumentsCount = 145;
        private const decimal price = 12.5m;
        private const int currencyId = 1;
    }
}
