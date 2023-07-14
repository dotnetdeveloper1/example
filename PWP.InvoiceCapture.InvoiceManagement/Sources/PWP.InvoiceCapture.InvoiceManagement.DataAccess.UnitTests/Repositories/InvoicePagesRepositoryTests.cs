using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class InvoicePageRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            cancellationToken = CancellationToken.None;
            context = (DatabaseContext)contextFactory.Create();
            target = new InvoicePageRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new InvoicePageRepository(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetListAsync_WhenInvoiceIdIsIncorrect_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetListAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetAsync_WhenPageIdIsIncorrect_ShouldThrowArgumentException(int pageId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(pageId, cancellationToken));
        }

        [TestMethod]
        [DataRow(100)]
        public async Task GetListAsync_WhenInvoicePagesCollectionIsNotEmpty_ShouldReturnAll(int count)
        {
            var invoicePages = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoicePage(index))
                .ToList();

            context.InvoicePages.AddRange(invoicePages);
            context.SaveChanges();

            var actualInvoicePages = await target.GetListAsync(invoiceId, cancellationToken);

            Assert.IsNotNull(actualInvoicePages);
            Assert.AreEqual(count, actualInvoicePages.Count);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetListAsync_WhenInvoicePagesCollectionIsEmpty_ShouldReturnEmptyList(int count)
        {
            var invoicePages = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoicePage(index))
                .ToList();

            context.InvoicePages.AddRange(invoicePages);
            context.SaveChanges();

            var actualInvoicePages = await target.GetListAsync(123123, cancellationToken);

            Assert.IsNotNull(actualInvoicePages);
            Assert.AreEqual(0, actualInvoicePages.Count);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetAsync_WhenInvoicePagesIsNotEmpty_ShouldReturnInvoicePage(int count)
        {
            var invoicePages = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoicePage(index))
                .ToList();

            context.InvoicePages.AddRange(invoicePages);
            context.SaveChanges();

            for (var pageId = 1; pageId <= count; pageId++)
            {
                var actualInvoicePage = await target.GetAsync(pageId, cancellationToken);

                AssertInvoicePagesAreEqual(invoicePages[pageId - 1], actualInvoicePage);
            }
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetAsync_WhenInvoicePageNotExists_ShouldReturnNull(int count)
        {
            var invoicePages = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoicePage(index))
                .ToList();

            context.InvoicePages.AddRange(invoicePages);
            context.SaveChanges();

            var actualInvoicePage = await target.GetAsync(123123, cancellationToken);

            Assert.IsNull(actualInvoicePage);
        }

        [TestMethod]
        public async Task CreateAsync_WhenPagesListIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_ShouldSavePages()
        {
            var pagesToSave = new List<InvoicePage>();
            pagesToSave.Add(CreateInvoicePage(1));
            pagesToSave.Add(CreateInvoicePage(2));
            await target.CreateAsync(pagesToSave, cancellationToken);
            var result = context.InvoicePages.ToList(); 

            Assert.AreEqual(result.Count, 2);
            AssertInvoicePagesAreEqual(pagesToSave[0], result[0]);
            AssertInvoicePagesAreEqual(pagesToSave[1], result[1]);
        }

        private InvoicePage CreateInvoicePage(int id) 
        {
            return new InvoicePage() 
            { 
                Id = id,
                ImageFileId = $"{id}.png",
                InvoiceId = invoiceId,
                Number = id,
                Width = 620,
                Height = 1280
            };
        }

        private void AssertInvoicePagesAreEqual(InvoicePage expected, InvoicePage actual) 
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ImageFileId, actual.ImageFileId);
            Assert.AreEqual(expected.InvoiceId, actual.InvoiceId);
            Assert.AreEqual(expected.Number, actual.Number);
            Assert.AreEqual(expected.Height, actual.Height);
            Assert.AreEqual(expected.Width, actual.Width);

            // Ensure all properties are tested
            Assert.AreEqual(6, actual.GetType().GetProperties().Length);
        }

        private DatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IInvoicePageRepository target;
        private CancellationToken cancellationToken;
        private const int invoiceId = 1;
    }
}
