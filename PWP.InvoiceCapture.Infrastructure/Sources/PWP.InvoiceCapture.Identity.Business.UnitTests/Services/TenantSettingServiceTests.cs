using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TenantSettingServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            tenantSettingRepositoryMock = mockRepository.Create<ITenantSettingRepository>();
            cultureRepositoryMock = mockRepository.Create<ICultureRepository>();
            target = new TenantSettingService(tenantSettingRepositoryMock.Object, cultureRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenTenantSettingRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantSettingService(null, cultureRepositoryMock.Object));
        }

        [TestMethod]
        public void Instance_WhenCultureRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantSettingService(tenantSettingRepositoryMock.Object, null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetAsync_WhenTenantIdIsZeroOrNegative_ShouldThrowArgumentException(int tenantId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenTenantSettingNotFound_ShouldReturnNull(int tenantId)
        {
            tenantSettingRepositoryMock
                .Setup(tenantSettingRepository => tenantSettingRepository.GetByTenantIdAsync(tenantId, cancellationToken))
                .ReturnsAsync((TenantSetting)null);

            var actualTenantSetting = await target.GetAsync(tenantId, cancellationToken);

            Assert.IsNull(actualTenantSetting);
        }

        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(11, 12)]
        public async Task GetAsync_WhenTenantSettingFound_ShouldReturnTenantSetting(int tenantId, int cultureId)
        {
            var expectedTenantSetting = CreateTenantSetting(tenantId, cultureId);

            tenantSettingRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetByTenantIdAsync(tenantId, cancellationToken))
                .ReturnsAsync(expectedTenantSetting);

            var actualTenantSetting = await target.GetAsync(tenantId, cancellationToken);

            Assert.AreEqual(expectedTenantSetting.TenantId, actualTenantSetting.TenantId);
            Assert.AreEqual(expectedTenantSetting.CultureId, actualTenantSetting.CultureId);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenTenantSettingRepositoryThrowError_ShouldThrowTimeoutException(int tenantId)
        {
            tenantSettingRepositoryMock
                .Setup(tenantSettingRepository => tenantSettingRepository.GetByTenantIdAsync(tenantId, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(2, 3)]
        public async Task CreateOrUpdateAsync_WhenTenantSettingWithTenantIdAlreadyExists_ShouldUpdate(int tenantId, int cultureId)
        {
            var tenantSetting = CreateTenantSetting(tenantId, cultureId);
            var culture = new Culture { Id = cultureId, Name = $"Name{cultureId}", EnglishName = $"EnglishName{cultureId}", NativeName = $"NativeName{cultureId}" };

            cultureRepositoryMock
                 .Setup(cultureRepository => cultureRepository.GetAsync(cultureId, cancellationToken))
                 .ReturnsAsync(culture);

            tenantSettingRepositoryMock
                .Setup(tenantSettingRepository => tenantSettingRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantSettingRepositoryMock
                .Setup(tenantSettingRepository => tenantSettingRepository.GetByTenantIdAsync(tenantId, cancellationToken))
                .ReturnsAsync(tenantSetting);

            tenantSettingRepositoryMock
                .Setup(tenantSettingRepository => tenantSettingRepository.UpdateAsync(tenantSetting, cancellationToken))
                .Returns(Task.CompletedTask);

            var actualResult = await target.CreateOrUpdateAsync(tenantId, cultureId, cancellationToken);

            Assert.IsTrue(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Success);
            Assert.AreEqual(actualResult.Message, $"TenantSetting with id = {tenantId} successfully created/updated.");
        }

        [TestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(4, 5, 6)]
        public async Task CreateOrUpdateAsync_WhenTenantSettingWithTenantIdDoesNotExist_ShouldCreate(int id, int tenantId, int cultureId)
        {
            var tenantSetting = CreateTenantSetting(tenantId, cultureId);
            var culture = new Culture { Id = cultureId, Name = $"Name{cultureId}", EnglishName = $"EnglishName{cultureId}", NativeName = $"NativeName{cultureId}" };

            var actualTenantSettings = new List<TenantSetting>();

            cultureRepositoryMock
                 .Setup(cultureRepository => cultureRepository.GetAsync(cultureId, cancellationToken))
                 .ReturnsAsync(culture);

            tenantSettingRepositoryMock
                .Setup(tenantSettingRepository => tenantSettingRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantSettingRepositoryMock
                .Setup(tenantSettingRepository => tenantSettingRepository.GetByTenantIdAsync(tenantId, cancellationToken))
                .ReturnsAsync((TenantSetting)null);

            tenantSettingRepositoryMock
                .Setup(tenantSettingRepository => tenantSettingRepository.CreateAsync(Capture.In(actualTenantSettings), cancellationToken))
                .ReturnsAsync(id);

            var actualResult = await target.CreateOrUpdateAsync(tenantId, cultureId, cancellationToken);

            Assert.IsTrue(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Success);
            Assert.AreEqual(actualResult.Message, $"TenantSetting with id = {tenantId} successfully created/updated.");
            Assert.AreEqual(1, actualTenantSettings.Count());
            AssertTenantSettingsAreEqual(tenantSetting, actualTenantSettings[0]);
        }


        private TenantSetting CreateTenantSetting(int tenantId, int cultureId)
        {
            return new TenantSetting
            {
                TenantId = tenantId,
                CultureId = cultureId
            };
        }

        private void AssertTenantSettingsAreEqual(TenantSetting actual, TenantSetting expected)
        {
            Assert.AreEqual(expected.TenantId, actual.TenantId);
            Assert.AreEqual(expected.CultureId, actual.CultureId);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.Id, actual.Id);

            Assert.AreEqual(5, actual.GetType().GetProperties().Length);
        }

        private MockRepository mockRepository;
        private Mock<ITenantSettingRepository> tenantSettingRepositoryMock;
        private Mock<ICultureRepository> cultureRepositoryMock;
        private TenantSettingService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
