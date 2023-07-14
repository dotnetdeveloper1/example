using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
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
    public class InvoiceRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            context = (DatabaseContext)contextFactory.Create();
            target = new InvoiceRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task CreateAsync_WhenInvoiceIsNotNull_ShouldSaveAndGenerateIdAndCreatedDateAndModifiedDate(int seed)
        {
            var expectedInvoice = CreateInvoice(seed);
            expectedInvoice.Id = 0;

            await target.CreateAsync(expectedInvoice, cancellationToken);

            var actualInvoice = context.Invoices.FirstOrDefault();

            Assert.IsNotNull(actualInvoice);
            Assert.AreNotEqual(0, actualInvoice.Id);
            Assert.AreNotEqual(default, actualInvoice.CreatedDate);
            Assert.AreNotEqual(default, actualInvoice.ModifiedDate);

            AssertInvoicesAreEqual(expectedInvoice, actualInvoice);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenInvoiceIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.UpdateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task UpdateAsync_WhenInvoiceIsNotNull_ShouldUpdateInvoiceAndModifiedDate(int seed)
        {
            var existingInvoice = CreateInvoice(seed);
            var invoiceId = existingInvoice.Id;

            context.Invoices.Add(existingInvoice);
            context.SaveChanges();

            var expectedInvoice = CreateInvoice(seed + 1);
            expectedInvoice.Id = invoiceId;

            await target.UpdateAsync(expectedInvoice, cancellationToken);

            var actualInvoice = context.Invoices.FirstOrDefault();

            Assert.IsNotNull(actualInvoice);
            Assert.AreNotEqual(default, actualInvoice.ModifiedDate);

            expectedInvoice.Name = existingInvoice.Name;
            expectedInvoice.FileId = existingInvoice.FileId;
            expectedInvoice.FileName = existingInvoice.FileName;
            expectedInvoice.CreatedDate = existingInvoice.CreatedDate;
            expectedInvoice.Status = existingInvoice.Status;
            expectedInvoice.FileSourceType = existingInvoice.FileSourceType;
            expectedInvoice.FromEmailAddress = existingInvoice.FromEmailAddress;

            AssertInvoicesAreEqual(expectedInvoice, actualInvoice);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task UpdateStatusAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateStatusAsync(invoiceId, InvoiceStatus.InProgress, cancellationToken));
        }

        [TestMethod]
        [DataRow(10, 1, InvoiceStatus.InProgress)]
        [DataRow(20, 2, InvoiceStatus.Queued)]
        [DataRow(30, 7, InvoiceStatus.Completed)]
        public async Task UpdateStatusAsync_WhenArgumentsAreValid_ShouldUpdateInvoiceStatusAndModifiedDate(int count, int invoiceId, InvoiceStatus expectedStatus)
        {
            var expectedInvoices = Enumerable
                .Range(1, count)
                .Select(index => CreateInvoice(index))
                .ToList();

            context.Invoices.AddRange(expectedInvoices);
            context.SaveChanges();

            await target.UpdateStatusAsync(invoiceId, expectedStatus, cancellationToken);

            var expectedInvoicesDictionary = expectedInvoices.ToDictionary(invoice => invoice.Id);
            var actualInvoices = context.Invoices.ToList();

            foreach (var actualInvoice in actualInvoices)
            {
                var expectedInvoice = expectedInvoicesDictionary[actualInvoice.Id];

                if (actualInvoice.Id == invoiceId)
                {
                    Assert.AreEqual(expectedStatus, actualInvoice.Status);
                    Assert.AreNotEqual(expectedInvoice.ModifiedDate, actualInvoice.ModifiedDate);
                    
                    // Setting properties which are different in expected and actual invoices to be able to make full invoice comparison
                    expectedInvoice.Status = actualInvoice.Status;
                    expectedInvoice.ModifiedDate = actualInvoice.ModifiedDate;
                }

                AssertInvoicesAreEqual(expectedInvoice, actualInvoice);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task UpdateStateAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateStateAsync(invoiceId, InvoiceState.Active, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, InvoiceState.Active)]
        [DataRow(2, InvoiceState.Archived)]
        [DataRow(7, InvoiceState.Deleted)]
        public async Task UpdateStateAsync_WhenArgumentsAreValid_ShouldUpdateInvoiceStateAndModifiedDate(int invoiceId, InvoiceState expectedState)
        {
            var expectedInvoice = CreateInvoice(invoiceId);

            context.Invoices.Add(expectedInvoice);
            context.SaveChanges();

            await target.UpdateStateAsync(invoiceId, expectedState, cancellationToken);
            var actualInvoices = context.Invoices.ToList();
            var actualInvoice = actualInvoices.Where(invoice => invoice.Id == invoiceId).FirstOrDefault();
            
            Assert.AreNotEqual(expectedInvoice.ModifiedDate, actualInvoice.ModifiedDate);
            Assert.AreEqual(expectedState, actualInvoice.InvoiceState);
           
            actualInvoice.ModifiedDate = expectedInvoice.ModifiedDate;
            actualInvoice.InvoiceState = expectedInvoice.InvoiceState;
          
            AssertInvoicesAreEqual(expectedInvoice, actualInvoice);
        }

        [TestMethod]
        public async Task GetListAsync_WhenInvoicesCollectionIsEmpty_ShouldReturnEmptyList()
        {
            var actualInvoices = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualInvoices);
            Assert.AreEqual(0, actualInvoices.Count);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task GetListAsync_WhenInvoicesCollectionIsNotEmpty_ShouldReturnAll(int expectedCount)
        {
            var expectedInvoices = Enumerable
                .Range(1, expectedCount)
                .Select(index => CreateInvoice(index))
                .ToList();

            context.Invoices.AddRange(expectedInvoices);
            context.SaveChanges();

            var actualInvoices = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualInvoices);
            Assert.AreEqual(expectedCount, actualInvoices.Count);

        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task GetListAsync_WhenInvoicesContainsDeletedFields_ShouldReturnInvoicesWithoutDeletedFields(int expectedCount)
        {
            var expectedInvoices = Enumerable
                .Range(1, expectedCount)
                .Select(index => CreateInvoice(index))
                .ToList();

            var group = CreateFieldGroup(1, 1);
            var field = CreateField(1, group.Id);
            var invoiceField = CreateInvoiceField(1, field.Id, expectedInvoices[0].Id);

            var deletedField = CreateField(2, group.Id);
            deletedField.IsDeleted = true;
            var deletedInvoiceField = CreateInvoiceField(2, deletedField.Id, expectedInvoices[0].Id);

            context.Invoices.AddRange(expectedInvoices);
            context.FieldGroups.Add(group);
            context.Fields.Add(field);
            context.Fields.Add(deletedField);
            context.InvoiceFields.Add(invoiceField);
            context.InvoiceFields.Add(deletedInvoiceField);
            context.SaveChanges();

            var actualInvoices = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualInvoices);
            Assert.AreEqual(expectedCount, actualInvoices.Count);

            var invoiceWithFields = actualInvoices.First(actualInvoice => actualInvoice.Id == expectedInvoices[0].Id);
            Assert.AreEqual(invoiceWithFields.InvoiceFields.Count(), 1);
        }

        [TestMethod]
        public async Task GetPaginatedListAsync_WhenPaginatedRequestIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
                target.GetPaginatedListAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task GetPaginatedListAsync_WhenInvoicesCollectionIsEmpty_ShouldReturnEmptyList()
        {
            var paginatedRequest = new InvoicePaginatedRequest();
            var actualResult = await target.GetPaginatedListAsync(paginatedRequest, cancellationToken);

            Assert.IsNotNull(actualResult);
            Assert.IsNotNull(actualResult.Items);
            Assert.AreEqual(0, actualResult.Items.Count);
            Assert.AreEqual(0, actualResult.TotalItemsCount);
        }

        [TestMethod]
        [DataRow(1, 10, 1, 1)]
        [DataRow(10, 6, 2, 4)]
        [DataRow(100, 15, 7, 10)]
        [DataRow(100, 30, 10, 0)]
        public async Task GetPaginatedListAsync_WhenInvoicesCollectionIsNotEmpty_ShouldReturnPaginatedResult(int totalCount, int itemsPerPage, int pageNumber, int expectedCount)
        {
            var paginatedRequest = new InvoicePaginatedRequest
            {
                PageNumber = pageNumber,
                ItemsPerPage = itemsPerPage
            };

            var expectedInvoices = Enumerable
                .Range(1, totalCount)
                .Select(index => CreateInvoice(index))
                .ToList();

            context.Invoices.AddRange(expectedInvoices);
            context.SaveChanges();

            var actualResult = await target.GetPaginatedListAsync(paginatedRequest, cancellationToken);

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(totalCount, actualResult.TotalItemsCount);
            Assert.IsNotNull(actualResult.Items);
            Assert.AreEqual(expectedCount, actualResult.Items.Count);

            if (actualResult.Items.Any())
            {
                var startId = (pageNumber - 1) * itemsPerPage + 1;
                var expectedIds = Enumerable.Range(startId, expectedCount).ToList();
                var actualIds = actualResult.Items.Select(invoice => invoice.Id).ToList();

                CollectionAssert.AreEquivalent(expectedIds, actualIds);
            }
        }

        [TestMethod]
        [DataRow(SortType.Ascending, InvoiceSortField.Id)]
        [DataRow(SortType.Descending, InvoiceSortField.Id)]
        [DataRow(SortType.Ascending, InvoiceSortField.FileId)]
        [DataRow(SortType.Descending, InvoiceSortField.FileId)]
        [DataRow(SortType.Ascending, InvoiceSortField.FileName)]
        [DataRow(SortType.Descending, InvoiceSortField.FileName)]
        [DataRow(SortType.Ascending, InvoiceSortField.FileSourceType)]
        [DataRow(SortType.Descending, InvoiceSortField.FileSourceType)]
        [DataRow(SortType.Ascending, InvoiceSortField.FromEmailAddress)]
        [DataRow(SortType.Descending, InvoiceSortField.FromEmailAddress)]
        [DataRow(SortType.Ascending, InvoiceSortField.InvoiceState)]
        [DataRow(SortType.Descending, InvoiceSortField.InvoiceState)]
        [DataRow(SortType.Ascending, InvoiceSortField.Name)]
        [DataRow(SortType.Descending, InvoiceSortField.Name)]
        [DataRow(SortType.Ascending, InvoiceSortField.Status)]
        [DataRow(SortType.Descending, InvoiceSortField.Status)]
        [DataRow(SortType.Ascending, InvoiceSortField.CreatedDate)]
        [DataRow(SortType.Descending, InvoiceSortField.CreatedDate)]
        [DataRow(SortType.Ascending, InvoiceSortField.ModifiedDate)]
        [DataRow(SortType.Descending, InvoiceSortField.ModifiedDate)]
        public async Task GetPaginatedListAsync_WhenInvoicesCollectionIsNotEmpty_ShouldReturnSortedResult(SortType type, InvoiceSortField sortField)
        {
            var paginatedRequest = new InvoicePaginatedRequest
            {
                SortType = type,
                SortBy = sortField
            };

            var expectedInvoices = Enumerable
                .Range(1, 10)
                .Select(index => CreateInvoice(index))
                .ToList();

            context.Invoices.AddRange(expectedInvoices);
            context.SaveChanges();

            var actualResult = await target.GetPaginatedListAsync(paginatedRequest, cancellationToken);

            Assert.IsNotNull(actualResult);
            Assert.IsNotNull(actualResult.Items);
            AssertCollectionIsSorted(actualResult.Items, type, sortField);
        }

        [TestMethod]
        public async Task GetActivePaginatedListAsync_WhenPaginatedRequestIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.GetActivePaginatedListAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task GetActivePaginatedListAsync_WhenInvoicesCollectionIsEmpty_ShouldReturnEmptyList()
        {
            var paginatedRequest = new InvoicePaginatedRequest();
            var actualResult = await target.GetActivePaginatedListAsync(paginatedRequest, cancellationToken);

            Assert.IsNotNull(actualResult);
            Assert.IsNotNull(actualResult.Items);
            Assert.AreEqual(0, actualResult.Items.Count);
            Assert.AreEqual(0, actualResult.TotalItemsCount);
        }

        [TestMethod]
        [DataRow(1, 10, 1, 1)]
        [DataRow(10, 6, 2, 4)]
        [DataRow(100, 15, 7, 10)]
        [DataRow(100, 30, 10, 0)]
        public async Task GetActivePaginatedListAsync_WhenInvoicesCollectionIsNotEmpty_ShouldReturnPaginatedResult(int totalCount, int itemsPerPage, int pageNumber, int expectedCount)
        {
            var paginatedRequest = new InvoicePaginatedRequest
            {
                PageNumber = pageNumber,
                ItemsPerPage = itemsPerPage
            };

            var expectedInvoices = Enumerable
                .Range(1, totalCount)
                .Select(index => CreateInvoice(index))
                .ToList();

            var notActiveInvoices = Enumerable
                .Range(totalCount + 1, totalCount)
                .Select(index => CreateInvoice(index, InvoiceState.Deleted))
                .ToList();

            context.Invoices.AddRange(expectedInvoices);
            context.Invoices.AddRange(notActiveInvoices);
            context.SaveChanges();

            var actualResult = await target.GetActivePaginatedListAsync(paginatedRequest, cancellationToken);

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(totalCount, actualResult.TotalItemsCount);
            Assert.IsNotNull(actualResult.Items);
            Assert.AreEqual(expectedCount, actualResult.Items.Count);

            if (actualResult.Items.Any())
            {
                var startId = (pageNumber - 1) * itemsPerPage + 1;
                var expectedIds = Enumerable.Range(startId, expectedCount).ToList();
                var actualIds = actualResult.Items.Select(invoice => invoice.Id).ToList();

                CollectionAssert.AreEquivalent(expectedIds, actualIds);
            }
        }

        [TestMethod]
        [DataRow(SortType.Ascending, InvoiceSortField.Id)]
        [DataRow(SortType.Descending, InvoiceSortField.Id)]
        [DataRow(SortType.Ascending, InvoiceSortField.FileId)]
        [DataRow(SortType.Descending, InvoiceSortField.FileId)]
        [DataRow(SortType.Ascending, InvoiceSortField.FileName)]
        [DataRow(SortType.Descending, InvoiceSortField.FileName)]
        [DataRow(SortType.Ascending, InvoiceSortField.FileSourceType)]
        [DataRow(SortType.Descending, InvoiceSortField.FileSourceType)]
        [DataRow(SortType.Ascending, InvoiceSortField.FromEmailAddress)]
        [DataRow(SortType.Descending, InvoiceSortField.FromEmailAddress)]
        [DataRow(SortType.Ascending, InvoiceSortField.InvoiceState)]
        [DataRow(SortType.Descending, InvoiceSortField.InvoiceState)]
        [DataRow(SortType.Ascending, InvoiceSortField.Name)]
        [DataRow(SortType.Descending, InvoiceSortField.Name)]
        [DataRow(SortType.Ascending, InvoiceSortField.Status)]
        [DataRow(SortType.Descending, InvoiceSortField.Status)]
        [DataRow(SortType.Ascending, InvoiceSortField.CreatedDate)]
        [DataRow(SortType.Descending, InvoiceSortField.CreatedDate)]
        [DataRow(SortType.Ascending, InvoiceSortField.ModifiedDate)]
        [DataRow(SortType.Descending, InvoiceSortField.ModifiedDate)]
        public async Task GetActivePaginatedListAsync_WhenInvoicesCollectionIsNotEmpty_ShouldReturnSortedResult(SortType type, InvoiceSortField sortField)
        {
            var paginatedRequest = new InvoicePaginatedRequest
            {
                SortType = type,
                SortBy = sortField
            };

            var expectedInvoices = Enumerable
                .Range(1, 10)
                .Select(index => CreateInvoice(index))
                .ToList();

            context.Invoices.AddRange(expectedInvoices);
            context.SaveChanges();

            var actualResult = await target.GetActivePaginatedListAsync(paginatedRequest, cancellationToken);

            Assert.IsNotNull(actualResult);
            Assert.IsNotNull(actualResult.Items);
            AssertCollectionIsSorted(actualResult.Items, type, sortField);
        }

        [TestMethod]
        public async Task GetActiveListAsync_WhenInvoicesCollectionIsEmpty_ShouldReturnEmptyList()
        {
            var actualInvoices = await target.GetActiveListAsync(cancellationToken);

            Assert.IsNotNull(actualInvoices);
            Assert.AreEqual(0, actualInvoices.Count);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task GetActiveListAsync_WhenInvoicesCollectionIsNotEmpty_ShouldReturnAll(int expectedCount)
        {
            var expectedInvoices = Enumerable
                .Range(1, expectedCount)
                .Select(index => CreateInvoice(index))
                .ToList();

            context.Invoices.AddRange(expectedInvoices);
            context.SaveChanges();

            var actualInvoices = await target.GetActiveListAsync(cancellationToken);

            Assert.IsNotNull(actualInvoices);
            Assert.AreEqual(expectedCount, actualInvoices.Count);

        }

        [TestMethod]
        public async Task GetActiveListAsync_WhenInvoicesCollectionIsNotEmpty_ShouldReturnActive()
        {
            var expectedInvoices = Enumerable
                .Range(1, 2)
                .Select(index => CreateInvoice(index))
                .ToList();
            expectedInvoices.Add(CreateInvoice(20, InvoiceState.Archived));
            expectedInvoices.Add(CreateInvoice(30, InvoiceState.Deleted));
            context.Invoices.AddRange(expectedInvoices);
            context.SaveChanges();

            var actualInvoices = await target.GetActiveListAsync(cancellationToken);

            Assert.IsNotNull(actualInvoices);
            Assert.AreEqual(2, actualInvoices.Count);
            Assert.AreEqual(InvoiceState.Active, actualInvoices[0].InvoiceState);
            Assert.AreEqual(InvoiceState.Active, actualInvoices[1].InvoiceState);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task GetActiveListAsync_WhenInvoicesContainsDeletedFields_ShouldReturnInvoicesWithoutDeletedFields(int expectedCount)
        {
            var expectedInvoices = Enumerable
                .Range(1, expectedCount)
                .Select(index => CreateInvoice(index))
                .ToList();

            var group = CreateFieldGroup(1, 1);
            var field = CreateField(1, group.Id);
            var invoiceField = CreateInvoiceField(1, field.Id, expectedInvoices[0].Id);

            var deletedField = CreateField(2, group.Id);
            deletedField.IsDeleted = true;
            var deletedInvoiceField = CreateInvoiceField(2, deletedField.Id, expectedInvoices[0].Id);

            context.Invoices.AddRange(expectedInvoices);
            context.FieldGroups.Add(group);
            context.Fields.Add(field);
            context.Fields.Add(deletedField);
            context.InvoiceFields.Add(invoiceField);
            context.InvoiceFields.Add(deletedInvoiceField);
            context.SaveChanges();

            var actualInvoices = await target.GetActiveListAsync(cancellationToken);

            Assert.IsNotNull(actualInvoices);
            Assert.AreEqual(expectedCount, actualInvoices.Count);

            var invoiceWithFields = actualInvoices.First(actualInvoice => actualInvoice.Id == expectedInvoices[0].Id);
            Assert.AreEqual(invoiceWithFields.InvoiceFields.Count(), 1);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenInvoiceContainceDeletedFields_ShouldReturnInvoiceWithoutDeletedFields(int invoiceId)
        {
            var expectedInvoice = CreateInvoice(invoiceId);

            var group = CreateFieldGroup(1, 1);
            var field = CreateField(1, group.Id);
            var invoiceField = CreateInvoiceField(1, field.Id, expectedInvoice.Id);

            var deletedField = CreateField(2, group.Id);
            deletedField.IsDeleted = true;
            var deletedInvoiceField = CreateInvoiceField(2, deletedField.Id, expectedInvoice.Id);

            context.Invoices.AddRange(expectedInvoice);
            context.FieldGroups.Add(group);
            context.Fields.Add(field);
            context.Fields.Add(deletedField);
            context.InvoiceFields.Add(invoiceField);
            context.InvoiceFields.Add(deletedInvoiceField);
            context.SaveChanges();

            Assert.AreEqual(1, context.Invoices.Count());

            var actualInvoice = await target.GetAsync(expectedInvoice.Id, cancellationToken);

            Assert.IsNotNull(actualInvoice);
            Assert.AreEqual(actualInvoice.InvoiceFields.Count(), 1);

            AssertInvoicesAreEqual(expectedInvoice, actualInvoice);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("      ")]
        public async Task GetAsync_WhenDocumentIdIsNull_ShouldThrowArgumentNullException(string documentId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.GetAsync(documentId, cancellationToken));
        }

        [TestMethod]
        [DataRow("fileId123", 1)]
        [DataRow("fileId123", 2)]
        public async Task GetAsync_WhenInvoiceWithDocumentIdNotFound_ShouldReturnNull(string documentId, int invoiceId)
        {
            context.Invoices.Add(CreateInvoice(invoiceId));
            context.SaveChanges();

            Assert.AreEqual(context.Invoices.Count(), 1);

            var actualInvoice = await target.GetAsync(documentId, cancellationToken);

            Assert.IsNull(actualInvoice);
        }

        [TestMethod]
        [DataRow("File1", 1)]
        [DataRow("File10", 10)]
        public async Task GetAsync_WhenInvoiceWithDocumentIdExists_ShouldReturnValidObject(string documentId, int invoiceId)
        {
            var expectedInvoice = CreateInvoice(invoiceId);

            context.Invoices.Add(expectedInvoice);
            context.SaveChanges();

            Assert.AreEqual(1, context.Invoices.Count());

            var actualInvoice = await target.GetAsync(documentId, cancellationToken);

            Assert.IsNotNull(actualInvoice);

            AssertInvoicesAreEqual(expectedInvoice, actualInvoice);
        }


        [TestMethod]
        [DataRow(5)]
        [DataRow(10)]
        public async Task GetAsync_WhenInvoiceWithIdNotFound_ShouldReturnNull(int invoiceId)
        {
            context.Invoices.Add(CreateInvoice(invoiceId - 1));
            context.SaveChanges();

            Assert.AreEqual(context.Invoices.Count(), 1);

            var actualInvoice = await target.GetAsync(invoiceId, cancellationToken);

            Assert.IsNull(actualInvoice);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenInvoiceWithIdExists_ShouldReturnValidObject(int invoiceId)
        {
            var expectedInvoice = CreateInvoice(invoiceId);

            context.Invoices.Add(expectedInvoice);
            context.SaveChanges();

            Assert.AreEqual(1, context.Invoices.Count());

            var actualInvoice = await target.GetAsync(expectedInvoice.Id, cancellationToken);

            Assert.IsNotNull(actualInvoice);

            AssertInvoicesAreEqual(expectedInvoice, actualInvoice);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetAsync_WhenInvoiceIdIsIncorrect_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task UpdateValidationMessageAsync_WhenInvoiceIdIsIncorrect_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.UpdateValidationMessageAsync(invoiceId, "123", cancellationToken));
        }

        [TestMethod]
        [DataRow(1, null)]
        [DataRow(15, "someMessage")]
        public async Task UpdateValidationMessageAsync_ShouldUpdate(int invoiceId, string message)
        {
            var invoiceIdNotForUpdate = invoiceId + 1;
            var invoiceForUpdate = CreateInvoice(invoiceId);
            var invoiceNotForUpdate = CreateInvoice(invoiceIdNotForUpdate);

            context.Invoices.Add(invoiceForUpdate);
            context.Invoices.Add(invoiceNotForUpdate);
            context.SaveChanges();

            await target.UpdateValidationMessageAsync(invoiceId, message, cancellationToken);

            Assert.IsNull(context.Invoices.FirstOrDefault(invoice => invoice.Id == invoiceIdNotForUpdate).ValidationMessage);
            Assert.AreEqual(message, context.Invoices.FirstOrDefault(invoice => invoice.Id == invoiceId).ValidationMessage);
        }

        private void AssertCollectionIsSorted(List<Invoice> invoices, SortType sortType, InvoiceSortField invoiceSortField)
        {
            if (invoices.Count < 2)
            {
                return;
            }

            for (var index = 1; index < invoices.Count; index++)
            {
                var argument1 = invoices[index - 1];
                var argument2 = invoices[index];

                switch (invoiceSortField)
                {
                    case InvoiceSortField.Id:
                        Assert.IsTrue(CompareSortedField(argument1.Id, argument2.Id, sortType));
                        break;
                    case InvoiceSortField.FileId:
                        Assert.IsTrue(CompareSortedField(argument1.FileId, argument2.FileId, sortType));
                        break;
                    case InvoiceSortField.FileName:
                        Assert.IsTrue(CompareSortedField(argument1.FileName, argument2.FileName, sortType));
                        break;
                    case InvoiceSortField.FileSourceType:
                        Assert.IsTrue(CompareSortedField(argument1.FileSourceType, argument2.FileSourceType, sortType));
                        break;
                    case InvoiceSortField.FromEmailAddress:
                        Assert.IsTrue(CompareSortedField(argument1.FromEmailAddress, argument2.FromEmailAddress, sortType));
                        break;
                    case InvoiceSortField.InvoiceState:
                        Assert.IsTrue(CompareSortedField(argument1.InvoiceState, argument2.InvoiceState, sortType));
                        break;
                    case InvoiceSortField.Name:
                        Assert.IsTrue(CompareSortedField(argument1.Name, argument2.Name, sortType));
                        break;
                    case InvoiceSortField.Status:
                        Assert.IsTrue(CompareSortedField(argument1.Status, argument2.Status, sortType));
                        break;
                    case InvoiceSortField.CreatedDate:
                        Assert.IsTrue(CompareSortedField(argument1.CreatedDate, argument2.CreatedDate, sortType));
                        break;
                    case InvoiceSortField.ModifiedDate:
                        Assert.IsTrue(CompareSortedField(argument1.ModifiedDate, argument2.ModifiedDate, sortType));
                        break;
                    default:
                        throw new ArgumentException("Unknown invoice sort field.");
                }
            }
        }

        private bool CompareSortedField<TEntity>(TEntity argument1, TEntity argument2, SortType type) where TEntity : IComparable
        {
            return type == SortType.Ascending
                ? argument2.CompareTo(argument1) >= 0
                : argument2.CompareTo(argument1) <= 0;
        }

        private FieldGroup CreateFieldGroup(int id, int orderNumber)
        {
            return new FieldGroup
            {
                Id = id,
                DisplayName = "DisplayName",
                OrderNumber = orderNumber
            };
        }

        private InvoiceField CreateInvoiceField(int id, int fieldId, int invoiceId)
        {
            return new InvoiceField
            {
                Id = id,
                Value = $"Value{id}",
                FieldId = fieldId,
                InvoiceId = invoiceId
            };
        }

        private Field CreateField(int id, int fieldGroupId)
        {
            return new Field
            {
                Id = id,
                GroupId = fieldGroupId,
                OrderNumber = 1,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                DefaultValue = $"DefaultValue{id}",
                DisplayName = $"DisplayName{id}",
                IsProtected = false,
                IsRequired = true,
                MaxLength = 10,
                MinLength = 1,
                MaxValue = 50,
                MinValue = 0,
                Type = FieldType.Decimal
            };
        }

        private Invoice CreateInvoice(int id, InvoiceState state = InvoiceState.Active)
        {
            var currentDate = DateTime.UtcNow;

            return new Invoice
            {
                Id = id,
                Name = $"Name{id}",
                FileId = $"File{id}",
                FileName = $"FileName{id}",
                FileSourceType = FileSourceType.API,
                Status = InvoiceStatus.NotStarted,
                InvoiceState = state,
                FromEmailAddress = $"FromEmailAddress{id}",
                CreatedDate = currentDate.AddSeconds(id),
                ModifiedDate = currentDate.AddSeconds(id)
            };
        }

        private InvoiceLine CreateInvoiceLine(int id, int invoiceId)
        {
            return new InvoiceLine()
            {
                Id = id,
                InvoiceId = invoiceId,
                OrderNumber = id,
                Number = "Number",
                Description = "Description",
                Price = 111,
                Quantity = 55,
                Total = 222
            };
        }

        private void AssertInvoicesAreEqual(Invoice expected, Invoice actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.FileId, actual.FileId);
            Assert.AreEqual(expected.FileName, actual.FileName);
            Assert.AreEqual(expected.FileSourceType, actual.FileSourceType);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.InvoiceState, actual.InvoiceState);
            Assert.AreEqual(expected.ValidationMessage, actual.ValidationMessage);
            Assert.AreEqual(expected.FromEmailAddress, actual.FromEmailAddress);

            // Ensure all properties are tested
            Assert.AreEqual(13, actual.GetType().GetProperties().Length);
        }

        private void AssertInvoiceDependencies(List<InvoiceLine> expectedLines, Invoice actualInvoice)
        {
            foreach (var actualLine in actualInvoice.InvoiceLines)
            {
                AssertInvoiceLinesAreEqual(expectedLines.First(line => actualLine.Id == line.Id), actualLine);
            }
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
        private IInvoiceRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
