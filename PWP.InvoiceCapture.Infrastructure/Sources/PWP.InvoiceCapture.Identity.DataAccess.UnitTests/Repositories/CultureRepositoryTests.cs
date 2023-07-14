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
    public class CultureRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new CultureRepository(contextFactory);
        }

        [TestMethod]
        public void Instance_WhenDbContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CultureRepository(null));
        }

        [TestMethod]
        public async Task GetListAsync_WhenCulturesCollectionIsEmpty_ShouldReturnEmptyListAsync()
        {
            var actualCultures = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualCultures);
            Assert.AreEqual(0, actualCultures.Count);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task GetListAsync_WhenCulturesCollectionIsNotEmpty_ShouldReturnAllAsync(int expectedCount)
        {
            var expectedCultures = Enumerable
                .Range(1, expectedCount)
                .Select(index => CreateCulture(index))
                .ToList();

            context.Cultures.AddRange(expectedCultures);
            context.SaveChanges();

            var actualCultures = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualCultures);
            Assert.AreEqual(expectedCount, actualCultures.Count);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetAsync_WhenCultureIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int cultureId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(cultureId, cancellationToken));
        }

        [TestMethod]
        [DataRow(5)]
        [DataRow(10)]
        public async Task GetAsync_WhenCultureWithIdNotFound_ShouldReturnNullAsync(int cultureId)
        {
            context.Cultures.Add(CreateCulture(cultureId - 1));
            context.SaveChanges();

            Assert.AreEqual(context.Cultures.Count(), 1);

            var actualCulture = await target.GetAsync(cultureId, cancellationToken);

            Assert.IsNull(actualCulture);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenCultureWithIdExists_ShouldReturnValidObjectAsync(int cultureId)
        {
            var expectedCulture = CreateCulture(cultureId);

            context.Cultures.Add(expectedCulture);
            context.SaveChanges();

            Assert.AreEqual(1, context.Cultures.Count());

            var actualCulture = await target.GetAsync(expectedCulture.Id, cancellationToken);

            Assert.IsNotNull(actualCulture);

            AssertCulturesAreEqual(expectedCulture, actualCulture);
        }

        private Culture CreateCulture(int id)
        {
            return new Culture
            {
                Id = id,
                Name = $"Name{id}",
                EnglishName = $"EnglishName{id}",
                NativeName = $"NativeName{id}",
            };
        }

        private void AssertCulturesAreEqual(Culture expected, Culture actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.EnglishName, actual.EnglishName);
            Assert.AreEqual(expected.NativeName, actual.NativeName);

            // Ensure all properties are tested
            Assert.AreEqual(4, actual.GetType().GetProperties().Length);
        }

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private ICultureRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
