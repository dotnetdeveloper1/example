using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PackServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            packRepositoryMock = mockRepository.Create<IPackRepository>();
            groupRepositoryMock = mockRepository.Create<IGroupRepository>();
            groupPackRepositoryMock = mockRepository.Create<IGroupPackRepository>();
            currencyRepositoryMock = mockRepository.Create<ICurrencyRepository>();

            target = new PackService(packRepositoryMock.Object, groupPackRepositoryMock.Object, groupRepositoryMock.Object, currencyRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenPackRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PackService(null, groupPackRepositoryMock.Object, groupRepositoryMock.Object, currencyRepositoryMock.Object));
        }

        [TestMethod]
        public void Instance_WhenGroupRepositoryMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PackService(packRepositoryMock.Object, groupPackRepositoryMock.Object, null, currencyRepositoryMock.Object));
        }

        [TestMethod]
        public void Instance_WhenGroupPackRepositoryMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PackService(packRepositoryMock.Object, null, groupRepositoryMock.Object, currencyRepositoryMock.Object));
        }

        [TestMethod]
        public void Instance_WhenCurrencyRepositoryMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PackService(packRepositoryMock.Object, groupPackRepositoryMock.Object, groupRepositoryMock.Object, null));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetGroupPackListAsync_WhenGroupIdIsWrong_ShouldThrowArgumentExceptionAsync(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetGroupPackListAsync(groupId, cancellationToken));
        }

        [TestMethod]
        public async Task GetGroupPackListAsync_WhenPackCollectionExists_ShouldReturnPacks()
        {
            var expectedGroupPacks = new List<GroupPack>() { new GroupPack { Id = groupPackId } };

            groupPackRepositoryMock
                .Setup(packRepository => packRepository.GetListAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedGroupPacks);

            var actualPacks = await target.GetGroupPackListAsync(groupId, cancellationToken);

            Assert.IsNotNull(actualPacks);
            Assert.AreEqual(actualPacks.Count(), 1);
            Assert.AreEqual(groupPackId, actualPacks[0].Id);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task CreateGroupPackAsync_WhenGroupIdIsWrong_ShouldThrowArgumentExceptionAsync(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.CreateGroupPackAsync(groupId, 1, cancellationToken));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task CreateAsync_WhenPackIdIsWrong_ShouldThrowArgumentExceptionAsync(int packId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.CreateGroupPackAsync(1, packId, cancellationToken));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetActiveGroupPackAsync_WhenGroupIdIsWrong_ShouldThrowArgumentExceptionAsync(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetActiveGroupPackAsync(groupId, cancellationToken));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetGroupPackByIdAsync_WhenPackIdIsWrong_ShouldThrowArgumentExceptionAsync(int packId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetGroupPackByIdAsync(packId, cancellationToken));
        }

        [TestMethod]
        public async Task CreateGroupPackAsync_WhenPackIsNotExists_ShouldReturnFailedOperationResultWithCorrespondingMessage()
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository
                .ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(true);
            packRepositoryMock
                .Setup(packRepository => packRepository
                .GetByIdAsync(packId, cancellationToken))
                .ReturnsAsync((Pack)null);

            var result = await target.CreateGroupPackAsync(groupId, packId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"Pack with id={packId} was not found", result.Message);
        }

        [TestMethod]
        public async Task CreateGroupPackAsync_WhenGroupIdIsNotExists_ShouldReturnFailedOperationResultWithCorrespondingMessage()
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(false);

            var result = await target.CreateGroupPackAsync(groupId, packId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"Group with id={groupId} was not found", result.Message);

        }

        [TestMethod]
        public async Task CreateGroupPackAsync_ShouldCreate()
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(true);

            packRepositoryMock
                .Setup(packRepository => packRepository.GetByIdAsync(packId, cancellationToken))
                .ReturnsAsync(new Pack() { });

            var actualGroupPack = new List<GroupPack>();

            groupPackRepositoryMock
                .Setup(groupPackRepository => groupPackRepository.CreateAsync(Capture.In(actualGroupPack), cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await target.CreateGroupPackAsync(groupId, packId, cancellationToken);

            Assert.AreEqual(packId, actualGroupPack[0].PackId);
            Assert.AreEqual(groupId, actualGroupPack[0].GroupId);
        }

        [TestMethod]
        public async Task GetGroupPackByIdAsync_ShouldCallRepository()
        {
            packRepositoryMock
                 .Setup(packRepository => packRepository.GetByIdAsync(packId, cancellationToken))
                 .ReturnsAsync(new Pack() { });

            var result = await target.GetGroupPackByIdAsync(packId, cancellationToken);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetActiveGroupPackAsync_ShouldCallRepository()
        {
            groupPackRepositoryMock
                 .Setup(groupPackRepository => groupPackRepository.GetActiveAsync(groupId, cancellationToken))
                 .ReturnsAsync(new List<GroupPack>() );

            var result = await target.GetActiveGroupPackAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task IncreaseCountOfUploadedInvoicesAsync_ShouldIncrease()
        {
            var groupPacks = new List<GroupPack>() { };
            groupPackRepositoryMock
                 .Setup(groupPackRepository => groupPackRepository.UpdateAsync(Capture.In(groupPacks), cancellationToken))
                 .Returns(Task.CompletedTask);
            
            var groupPackToUpdate = new GroupPack()
            {
                Id = groupPackId,
                UploadedDocumentsCount = 0
            };
            await target.IncreaseCountOfUploadedInvoices(groupPackToUpdate, cancellationToken);

            Assert.AreEqual(1, groupPacks.Count);
            Assert.AreEqual(1, groupPacks[0].UploadedDocumentsCount);
        }

        [TestMethod]
        public async Task GetListAsync_ShouldCallRepository()
        {
            packRepositoryMock
                 .Setup(packRepository => packRepository.GetListAsync(cancellationToken))
                 .ReturnsAsync(new List<Pack>() { });

            var result = await target.GetPackListAsync(cancellationToken);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetPackListAsync_ShouldCallRepository()
        {
            packRepositoryMock
                 .Setup(packRepository => packRepository.GetListAsync(cancellationToken))
                 .ReturnsAsync(new List<Pack>() { });

            var result = await target.GetPackListAsync(cancellationToken);

            Assert.IsNotNull(result);
        }

        [DataRow(null)]
        [DataRow("")]
        [TestMethod]
        public async Task CreateAsync_WhenPackNameIsEmpty_ShouldReturnFailedOperationResultWithCorrespondingMessage(string packName)
        {
            var result = await target.CreatePackAsync(new PackCreationParameters() { PackName = packName }, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"Name required.", result.Message);
        }


        [TestMethod]
        public async Task CreateAsync_WhenAllowedDocumentsCountIsNegative_ShouldReturnFailedOperationResultWithCorrespondingMessage()
        {
            var result = await target.CreatePackAsync(
                new PackCreationParameters() { PackName  = testPackName, AllowedDocumentsCount = -1 }, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"AllowedDocumentsCount is less then zero.", result.Message);
        }

        [TestMethod]
        public async Task CreateAsync_WhenCurrencyIsNotExists_ShouldReturnFailedOperationResultWithCorrespondingMessage()
        {
            currencyRepositoryMock
                .Setup(currencyRepository => currencyRepository.ExistsAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(false);

            var result = await target.CreatePackAsync(
                new PackCreationParameters() { PackName = testPackName, AllowedDocumentsCount = allowedDocumentsCount, CurrencyId = currencyId }, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"Currency with id {currencyId} is not exists.", result.Message);
        }

        [TestMethod]
        public async Task CreateAsync_WhenParamsAreOk_ShouldCallRepository()
        {
            var actualPacks = new List<Pack>();
            currencyRepositoryMock
                 .Setup(currencyRepository => currencyRepository.ExistsAsync(It.IsAny<int>(), cancellationToken))
                 .ReturnsAsync(true);

            packRepositoryMock
                 .Setup(packRepository => packRepository.CreateAsync(Capture.In(actualPacks), cancellationToken))
                 .ReturnsAsync(It.IsAny<int>());

            var result = await target.CreatePackAsync(
                new PackCreationParameters() { PackName = testPackName, AllowedDocumentsCount = allowedDocumentsCount, CurrencyId = currencyId, Price = price }, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccessful);
            Assert.AreEqual(1, actualPacks.Count);
            var actualPack = actualPacks[0];
            Assert.AreEqual(testPackName, actualPack.Name);
            Assert.AreEqual(allowedDocumentsCount, actualPack.AllowedDocumentsCount);
            Assert.AreEqual(price, actualPack.Price);
        }

        [DataRow(3)]
        [DataRow(16)]
        [TestMethod]
        public async Task DeleteGroupPackByIdAsync_ShouldDelete(int groupPackId)
        {
            groupPackRepositoryMock
                 .Setup(groupPack => groupPack.GetByIdAsync(groupPackId, cancellationToken))
                 .ReturnsAsync(new GroupPack() { Id = groupPackId });

            groupPackRepositoryMock
                 .Setup(groupPack => groupPack.DeleteAsync(groupPackId, cancellationToken))
                 .Returns(Task.CompletedTask);

            var result = await target.DeleteGroupPackByIdAsync(groupPackId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccessful);
        }

        [DataRow(3)]
        [DataRow(16)]
        [TestMethod]
        public async Task DeleteGroupPackByIdAsync_WhenThereIsNoGroupPack_ShouldReturnNotFoundResult(int groupPackId)
        {
            groupPackRepositoryMock
                 .Setup(groupPack => groupPack.GetByIdAsync(groupPackId, cancellationToken))
                 .ReturnsAsync((GroupPack)null);

            var result = await target.DeleteGroupPackByIdAsync(groupPackId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.NotFound, result.Status);
            Assert.AreEqual($"There is no groupPack with id = '{groupPackId}'.", result.Message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task DeleteGroupPackByIdAsync_WhenGroupPackIdIsWrong_ShouldThrowArgumentException(int groupPackId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteGroupPackByIdAsync(groupPackId, cancellationToken));
        }

        private MockRepository mockRepository;
        private Mock<IPackRepository> packRepositoryMock;
        private Mock<IGroupRepository> groupRepositoryMock;
        private Mock<IGroupPackRepository> groupPackRepositoryMock;
        private Mock<ICurrencyRepository> currencyRepositoryMock;
        private PackService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int groupId = 14;
        private const int packId = 16;
        private const int groupPackId = 26;
        private const string testPackName = "somePackName";
        private const int currencyId = 5;
        private const int allowedDocumentsCount = 55;
        private const decimal price = 66.78m;
    }
}
