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
    public class PackRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new PackRepository(contextFactory);
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
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetByIdAsync_PackIdIsWrong_ShouldThrowArgumentExceptionAsync(int packId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetByIdAsync(packId, cancellationToken));
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnPackAsync()
        {
            context.Packs.Add(new Pack() { Id = packId, CurrencyId = currencyId });
            context.Packs.Add(new Pack() { Id = packId2, CurrencyId = currencyId });
            context.Packs.Add(new Pack() { Id = packId3, CurrencyId = currencyId });
            context.Currencies.Add(new Currency() { Id = currencyId });
            context.SaveChanges();

            var actualPack = await target.GetByIdAsync(packId, cancellationToken);

            Assert.IsNotNull(actualPack);
            Assert.AreEqual(packId, actualPack.Id);
        }

        [TestMethod]
        public async Task GetListAsync_ShouldReturnPackAsync()
        {
            context.Packs.Add(new Pack() { Id = packId, CurrencyId = currencyId });
            context.Packs.Add(new Pack() { Id = packId2, CurrencyId = currencyId });
            context.Packs.Add(new Pack() { Id = packId3, CurrencyId = currencyId });
            context.Currencies.Add(new Currency() { Id = currencyId });
            context.SaveChanges();

            var actualPacks = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualPacks);
            Assert.AreEqual(3, actualPacks.Count);
            Assert.AreEqual(packId, actualPacks[0].Id);
            Assert.AreEqual(currencyId, actualPacks[0].CurrencyId);
            Assert.AreEqual(packId, actualPacks[0].Id);
            Assert.AreEqual(packId2, actualPacks[1].Id);
            Assert.AreEqual(packId3, actualPacks[2].Id);
        }

        [TestMethod]
        public async Task CreateAsync_WhenPackNotNull_ShouldSavePackAsync()
        {
            var expectedPack = new Pack()
            {
                Name = packName,
                AllowedDocumentsCount = allowedDocumentsCount,
                CurrencyId = currencyId,
                Price = price
            };

            await target.CreateAsync(expectedPack, cancellationToken);

            var actualPack = context.Packs.FirstOrDefault();

            Assert.IsNotNull(actualPack);
            Assert.AreNotEqual(0, actualPack.Id);
            Assert.AreNotEqual(default, actualPack.CreatedDate);
            Assert.AreNotEqual(default, actualPack.ModifiedDate);

            Assert.AreEqual(packName, actualPack.Name);
            Assert.AreEqual(price, actualPack.Price);
            Assert.AreEqual(allowedDocumentsCount, actualPack.AllowedDocumentsCount);
            Assert.AreEqual(currencyId, actualPack.CurrencyId);
        }

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private IPackRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int packId = 22;
        private const int packId2 = 2;
        private const int packId3 = 33;
        private const int currencyId = 11;
        private const string packName = "somePackName";
        private const int allowedDocumentsCount = 111;
        private const decimal price = 17.5m;
    }
}
