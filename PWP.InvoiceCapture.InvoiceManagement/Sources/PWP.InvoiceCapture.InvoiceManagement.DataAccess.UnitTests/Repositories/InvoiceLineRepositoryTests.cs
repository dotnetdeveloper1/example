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
    public class InvoiceLineRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            cancellationToken = CancellationToken.None;
            context = (DatabaseContext)contextFactory.Create();
            target = new InvoiceLineRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceLineRepository(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetListAsync_WhenInvoiceIdIsZeroOrLess_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetListAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetAsync_WhenInvoiceLineIdIsZeroOrLess_ShouldThrowArgumentException(int invoiceLineId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(invoiceLineId, cancellationToken));
        }

        [TestMethod]
        [DataRow(100)]
        public async Task GetListAsync_WhenInvoiceLinesCollectionIsNotEmpty_ShouldReturnAll(int count)
        {
            var invoiceLines = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoiceLine(index))
                .ToList();

            context.InvoiceLines.AddRange(invoiceLines);
            context.SaveChanges();

            var actualInvoiceLines = await target.GetListAsync(invoiceId, cancellationToken);

            Assert.IsNotNull(actualInvoiceLines);
            Assert.AreEqual(count, actualInvoiceLines.Count);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetListAsync_WhenInvoiceIdNotExists_ShouldReturnEmptyList(int count)
        {
            var invoiceLines = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoiceLine(index))
                .ToList();

            context.InvoiceLines.AddRange(invoiceLines);
            context.SaveChanges();

            var actualInvoiceLines = await target.GetListAsync(notExistingInvoiceId, cancellationToken);

            Assert.IsNotNull(actualInvoiceLines);
            Assert.AreEqual(0, actualInvoiceLines.Count);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetAsync_WhenInvoiceLineIdExists_ShouldReturnInvoiceLine(int count)
        {
            var invoiceLines = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoiceLine(index))
                .ToList();

            context.InvoiceLines.AddRange(invoiceLines);
            context.SaveChanges();

            for (var invoiceLineId = 1; invoiceLineId <= count; invoiceLineId++)
            {
                var actualInvoicePage = await target.GetAsync(invoiceLineId, cancellationToken);

                AssertInvoiceLinesAreEqual(invoiceLines[invoiceLineId - 1], actualInvoicePage);
            }
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetAsync_WhenInvoiceLineNotExists_ShouldReturnNull(int count)
        {
            var invoiceLines = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoiceLine(index))
                .ToList();

            context.InvoiceLines.AddRange(invoiceLines);
            context.SaveChanges();

            var actualInvoiceLine = await target.GetAsync(notExistingInvoiceId, cancellationToken);

            Assert.IsNull(actualInvoiceLine);
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceLineIsNull_ShouldThrowArgumentNullException()
        {
            InvoiceLine invoiceLine = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(invoiceLine, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceLineListIsNull_ShouldThrowArgumentNullException()
        {
            List<InvoiceLine> invoiceLines = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(invoiceLines, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceLineListIsEmpty_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(new List<InvoiceLine>(), cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_ShouldSaveInvoiceLines()
        {
            var invoiceLines = new List<InvoiceLine>();
            invoiceLines.Add(CreateInvoiceLine(1));
            invoiceLines.Add(CreateInvoiceLine(2));

            await target.CreateAsync(invoiceLines, cancellationToken);

            var result = context.InvoiceLines.ToList();

            Assert.AreEqual(result.Count, 2);

            AssertInvoiceLinesAreEqual(invoiceLines[0], result[0]);
            AssertInvoiceLinesAreEqual(invoiceLines[1], result[1]);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldSaveInvoiceLine()
        {
            var invoiceLine = CreateInvoiceLine(1);

            await target.CreateAsync(invoiceLine, cancellationToken);

            var result = context.InvoiceLines.ToList();

            Assert.AreEqual(result.Count, 1);

            AssertInvoiceLinesAreEqual(invoiceLine, result[0]);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteByInvoiceIdAsync_WhenInvoiceIdIsZeroOrLessThenZero_ShouldReturnArgumentException(int incorrectInvoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteByInvoiceIdAsync(incorrectInvoiceId, cancellationToken));
        }

        [TestMethod]
        public async Task DeleteByInvoiceIdAsync_WhenInvoiceLinesForInvoiceExists_ShouldDeleteAllInvoiceLinesForInvoice()
        {
            var invoiceLines = new List<InvoiceLine>();
            invoiceLines.Add(CreateInvoiceLine(1));
            invoiceLines.Add(CreateInvoiceLine(2));
            invoiceLines.Add(CreateInvoiceLine(3));
            invoiceLines[2].InvoiceId = 123;

            context.InvoiceLines.AddRange(invoiceLines);
            await context.SaveChangesAsync();

            await target.DeleteByInvoiceIdAsync(invoiceId, cancellationToken);

            var result = context.InvoiceLines.ToList();

            Assert.AreEqual(result.Count, 1);
        }

        private InvoiceLine CreateInvoiceLine(int id)
        {
            return new InvoiceLine()
            {
                Id = id,
                InvoiceId = invoiceId,
                OrderNumber = id,
                Number = "invoiceLineNumber",
                Description = "invoiceLineDescription",
                Price = 100,
                Quantity = 12,
                Total = 5
            };
        }

        private void AssertInvoiceLinesAreEqual(InvoiceLine expected, InvoiceLine actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.InvoiceId, actual.InvoiceId);
            Assert.AreEqual(expected.OrderNumber, actual.OrderNumber);
            Assert.AreEqual(expected.Number, actual.Number);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.Quantity, actual.Quantity);
            Assert.AreEqual(expected.Total, actual.Total);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);

            // Ensure all properties are tested
            Assert.AreEqual(10, actual.GetType().GetProperties().Length);
        }

        private DatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IInvoiceLineRepository target;
        private CancellationToken cancellationToken;

        private const int invoiceId = 1;
        private const int notExistingInvoiceId = 123123;
    }
}
