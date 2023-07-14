using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CurrencyRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            cancellationToken = CancellationToken.None;
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new CurrencyRepository(contextFactory);
        }

        [TestMethod]
        public void Instance_WhenDbContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CurrencyRepository(null));
        }

        [TestMethod]
        [DataRow(5)]
        [DataRow(10)]
        public async Task ExistsAsync_WhenCurrencyWithIdNotFound_ShouldReturnFalseAsync(int currencyId)
        {
            context.Currencies.Add(CreateCurrency(currencyId - 1));
            context.SaveChanges();

            Assert.AreEqual(context.Currencies.Count(), 1);

            var currencyExists = await target.ExistsAsync(currencyId, cancellationToken);

            Assert.IsFalse(currencyExists);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task ExistsAsync_WhenCurrencyWithIdExists_ShouldReturnTrueAsync(int currencyId)
        {
            var expectedCurrency = CreateCurrency(currencyId);

            context.Currencies.Add(expectedCurrency);

            context.SaveChanges();

            Assert.AreEqual(1, context.Currencies.Count());

            var isCurrencyExists = await target.ExistsAsync(expectedCurrency.Id, cancellationToken);

            Assert.IsTrue(isCurrencyExists);
        }

        private Currency CreateCurrency(int currencyId)
        {
            return new Currency
            {
                Id = currencyId
            };
        }

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private ICurrencyRepository target;
        private CancellationToken cancellationToken;
    }
}
