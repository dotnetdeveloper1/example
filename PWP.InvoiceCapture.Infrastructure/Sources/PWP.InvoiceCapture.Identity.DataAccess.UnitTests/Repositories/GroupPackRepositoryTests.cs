using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.UnitTests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.TenantCapture.Identity.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class GroupPackRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new GroupPackRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new PackRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenGroupPackIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task CreateAsync_WhenGroupPackNotNull_ShouldSaveGroupPackAsync(int groupPackId)
        {
            var expectedGroupPack = CreateGroupPack(groupPackId);

            await target.CreateAsync(expectedGroupPack, cancellationToken);

            var actualGroupPack = context.GroupPacks.FirstOrDefault();

            Assert.IsNotNull(actualGroupPack);
            Assert.AreNotEqual(0, actualGroupPack.Id);
            Assert.AreNotEqual(default, actualGroupPack.CreatedDate);
            Assert.AreNotEqual(default, actualGroupPack.ModifiedDate);

            AssertPacksAreEqual(expectedGroupPack, actualGroupPack);
        }

        [TestMethod]
        public async Task GetListAsync_WhenPackCollectionIsEmpty_ShouldReturnEmptyListAsync()
        {
            var actualPacks = await target.GetListAsync(1, cancellationToken);

            Assert.IsNotNull(actualPacks);
            Assert.AreEqual(0, actualPacks.Count);
        }

        [TestMethod]
        public async Task GetListAsync_WhenPacksCollectionIsNotEmpty_ShouldReturnAllAsync()
        {
            var groupPack = CreateGroupPack(1);
            context.GroupPacks.Add(groupPack);
            context.Groups.Add(new Group() { Id = groupId});
            context.Packs.Add(new Pack() { Id = packId, CurrencyId = 1});
            context.Currencies.Add(new Currency() { Id = 1});
            context.SaveChanges();

            var actualPacks = await target.GetListAsync(groupId, cancellationToken);

            Assert.IsNotNull(actualPacks);
            Assert.AreEqual(1, actualPacks.Count);
            AssertPacksAreEqual(groupPack, actualPacks[0]);
        }

        [DataRow(-1)]
        [DataRow(0)]
        [TestMethod]
        public async Task GetListAsync_WhenPackIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int packId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetListAsync(packId, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsyncc_GroupPackIdIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task UpdateAsync_GroupPackIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldIncreaseAsync()
        {
            var groupPack = new GroupPack()
            {
                Id = groupPackId,
                UploadedDocumentsCount = uploadedCount
            };
            var expectedCount = uploadedCount + 1;
            context.GroupPacks.Add(groupPack);
            context.SaveChanges();
            groupPack.UploadedDocumentsCount = expectedCount;
            await target.UpdateAsync(groupPack, cancellationToken);

            Assert.AreEqual(expectedCount, context.GroupPacks.FirstOrDefault(groupPack => groupPack.Id == groupPackId).UploadedDocumentsCount);
        }

        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-5)]
        [TestMethod]
        public async Task GetByIdAsync_GroupPackIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int groupPackId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetByIdAsync(groupPackId, cancellationToken));
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnGroupPackAsync()
        {
            var groupPacks = Enumerable
                .Range(1, 3)
                .Select(index => CreateGroupPack(index))
                .ToList();

            context.Groups.Add(new Group() { Id = groupId });
            context.Currencies.Add(new Currency() { Id = currencyId });
            context.Packs.Add(new Pack() { Id = packId, CurrencyId = currencyId });
            context.GroupPacks.AddRange(groupPacks);
            context.SaveChanges();

            var result = await target.GetByIdAsync(2, cancellationToken);

            Assert.IsNotNull(result);
            AssertPacksAreEqual(context.GroupPacks.ToArray()[1], result);
        }

        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-5)]
        [TestMethod]
        public async Task DeleteAsync_GroupPackdIsIncorrect_ShouldThrowArgumentExceptionAsync(int groupPackId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(groupPackId, cancellationToken));
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteAsync()
        {
            var groupPacks = Enumerable
                .Range(1, 3)
                .Select(index => CreateGroupPack(index))
                .ToList();
            context.GroupPacks.AddRange(groupPacks);
            context.SaveChanges();

            await target.DeleteAsync(groupPacks.FirstOrDefault(groupPacks => groupPacks.Id == 2).Id, cancellationToken);

            Assert.AreEqual(2, context.GroupPacks.Count());
            Assert.AreEqual(1, context.GroupPacks.ToArray()[0].Id);
            Assert.AreEqual(3, context.GroupPacks.ToArray()[1].Id);
        }

        [TestMethod]
        public async Task GetActiveAsync_ShouldReturnActiveAsync()
        {
            context.Packs.Add(new Pack() { Id = packId, CurrencyId = 1, AllowedDocumentsCount = 10 });
            context.Currencies.Add(new Currency() { Id = 1});
            context.Groups.Add(new Group() { Id = groupId });
            context.GroupPacks.Add(new GroupPack()
            {
                Id = 1,
                UploadedDocumentsCount = 8,
                PackId = packId,
                GroupId = groupId
            });
            context.GroupPacks.Add(new GroupPack()
            {
                Id = 2,
                UploadedDocumentsCount = 8,
                PackId = packId,
                GroupId = 55
            });
            context.GroupPacks.Add(new GroupPack()
            {
                Id = 3,
                UploadedDocumentsCount = 10,
                PackId = packId,
                GroupId = groupId

            });
            context.GroupPacks.Add(new GroupPack()
            {
                Id = 4,
                UploadedDocumentsCount = 11,
                PackId = packId,
                GroupId = groupId
            });

            context.SaveChanges();

            var result = await target.GetActiveAsync(groupId,
               cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Id);
        }

        private GroupPack CreateGroupPack(int id)
        {
            return new GroupPack
            {
                Id = id,
                PackId = packId,
                GroupId = groupId,
                UploadedDocumentsCount = 0,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }

        private void AssertPacksAreEqual(GroupPack expected, GroupPack actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.PackId, actual.PackId);
            Assert.AreEqual(expected.GroupId, actual.GroupId);
            Assert.AreEqual(expected.UploadedDocumentsCount, actual.UploadedDocumentsCount);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
        }

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private IGroupPackRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int packId = 22;
        private const int groupId = 33;
        private const int groupPackId = 44;
        private const int currencyId = 55;
        private const int uploadedCount = 20;

    }
}
