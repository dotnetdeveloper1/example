using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class TenantSettingRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new TenantSettingRepository(contextFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public void Instance_WhenDbContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantSettingRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenTenantSettingIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(10, 11, 12)]
        [DataRow(100, 101, 102)]
        public async Task CreateAsync_WhenTenantSettingIsNotNull_ShouldSaveTenantSettingAsync(int id, int tenantId, int cultureId)
        {
            var expectedTenantSetting = CreateTenantSetting(id, tenantId, cultureId);
            expectedTenantSetting.Id = 0;

            await target.CreateAsync(expectedTenantSetting, cancellationToken);

            var actualTenantSetting = context.TenantSettings.FirstOrDefault();

            Assert.IsNotNull(actualTenantSetting);
            Assert.AreNotEqual(0, actualTenantSetting.Id);
            Assert.AreNotEqual(default, actualTenantSetting.CreatedDate);
            Assert.AreNotEqual(default, actualTenantSetting.ModifiedDate);

            AssertTenantSettingsAreEqual(expectedTenantSetting, actualTenantSetting);
        }

        [TestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(4, 5, 6)]
        public async Task GetByTenantIdAsync_WhenTenantSettingWithTenantIdNotFound_ShouldReturnNullAsync(int id, int tenantId, int cultureId)
        {
            context.TenantSettings.Add(CreateTenantSetting(id, tenantId - 1, cultureId));
            context.SaveChanges();

            Assert.AreEqual(context.TenantSettings.Count(), 1);

            var actualTenantSetting = await target.GetByTenantIdAsync(tenantId, cancellationToken);

            Assert.IsNull(actualTenantSetting);
        }

        [TestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(4, 5, 6)]
        public async Task GetByTenantIdAsync_WhenTenantSettingWithTenantIdExists_ShouldReturnValidObjectAsync(int id, int tenantId, int cultureId)
        {
            var expectedTenantSetting = CreateTenantSetting(id, tenantId, cultureId);

            context.TenantSettings.Add(expectedTenantSetting);
            context.SaveChanges();

            Assert.AreEqual(1, context.TenantSettings.Count());

            var actualTenantSetting = await target.GetByTenantIdAsync(expectedTenantSetting.TenantId, cancellationToken);

            Assert.IsNotNull(actualTenantSetting);

            AssertTenantSettingsAreEqual(expectedTenantSetting, actualTenantSetting);
        }
        
        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetByTenantIdAsync_WhenTenantIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int tenantId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetByTenantIdAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenTenantSettingIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            TenantSetting tenantSetting = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(tenantSetting, cancellationToken));
        }
        
        [TestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(10, 20, 30)]
        public async Task UpdateAsync_WhenArgumentsAreValid_ShouldUpdateTenantSettingAsync(
            int id,
            int tenantId,
            int cultureId)
        {
            var tenantSetting = CreateTenantSetting(id, tenantId, 99);

            context.TenantSettings.Add(tenantSetting);
            context.SaveChanges();

            tenantSetting.CultureId = cultureId;

            await target.UpdateAsync(tenantSetting, cancellationToken);

            var actualTenantSetting = context.TenantSettings.FirstOrDefault();

            Assert.AreEqual(actualTenantSetting.CultureId, cultureId);
        }

        private TenantSetting CreateTenantSetting(int id, int tenantId, int cultureId)
        {
            return new TenantSetting
            {
                Id = id,
                TenantId = tenantId,
                CultureId = cultureId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
            };
        }

        private void AssertTenantSettingsAreEqual(TenantSetting expected, TenantSetting actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.TenantId, actual.TenantId);
            Assert.AreEqual(expected.CultureId, actual.CultureId);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);

            // Ensure all properties are tested
            Assert.AreEqual(5, actual.GetType().GetProperties().Length);
        }

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private ITenantSettingRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
