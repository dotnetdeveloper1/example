using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class InvoiceProcessingResultRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            context = (DatabaseContext)contextFactory.Create();
            target = new InvoiceProcessingResultRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceProcessingResultRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenProcessingResultIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetAsync_WhenProcessingResultIdIsIncorrect_ShouldThrowArgumentException(int processingResultId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetAsync(processingResultId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetByInvoiceIdAsync_WhenInvoiceIdIsIncorrect_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetByInvoiceIdAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetLastByInvoiceIdAsync_WhenInvoiceIdIsIncorrect_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetLastByInvoiceIdAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-10)]
        public async Task CreateAsync_WhenInvoiceIdIsInvalid_ShouldThrowArgumentException(int invoiceId)
        {
            var processingResult = CreateInvoiceProcessingResult(0);
            processingResult.InvoiceId = invoiceId;

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.CreateAsync(processingResult, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetAsync_WhenInvoiceProcessingResultIdIsInvalid_ShouldThrowArgumentException(int invoiceProcessingResultId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetAsync(invoiceProcessingResultId, cancellationToken));
        }

        [TestMethod]
        [DataRow(10, 12)]
        [DataRow(1, 17)]
        public async Task GetInvoiceIdAsync_WhenInvoiceProcessingResultExists_ShouldReturnInvoiceId(int processingResultId, int invoiceId)
        {
            var expectedProcessingResult = CreateInvoiceProcessingResult(processingResultId, invoiceId);
            context.InvoiceProcessingResults.Add(expectedProcessingResult);
            context.SaveChanges();

            var actualInvoiceId = await target.GetInvoiceIdAsync(processingResultId, cancellationToken);

            Assert.IsNotNull(actualInvoiceId);
            Assert.AreEqual(invoiceId, actualInvoiceId.Value);
        }

        [TestMethod]
        [DataRow(10)]
        [DataRow(1)]
        public async Task GetInvoiceIdAsync_WhenInvoiceProcessingResultDoesNotExist_ShouldReturnNull(int processingResultId)
        {
            var invoiceId = await target.GetInvoiceIdAsync(processingResultId, cancellationToken);

            Assert.IsNull(invoiceId);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task CreateAsync_WhenProcessingResultIsNotNull_ShouldSaveAndGenerateId(int seed)
        {
            var expectedProcessingResult = CreateInvoiceProcessingResult(seed);
            expectedProcessingResult.Id = 0;

            await target.CreateAsync(expectedProcessingResult, cancellationToken);

            var actualProcessingResult = context.InvoiceProcessingResults.FirstOrDefault();

            Assert.IsNotNull(actualProcessingResult);
            Assert.AreNotEqual(0, actualProcessingResult.Id);
            AssertInvoiceProcessingResultsAreEqual(expectedProcessingResult, actualProcessingResult);
        }

        [TestMethod]
        public async Task CreateAsync_WhenCreatedAndModifiedDatesAreNotProvided_ShouldSaveWithCurrentDate()
        {
            var expectedProcessingResult = CreateInvoiceProcessingResult(0);

            await target.CreateAsync(expectedProcessingResult, cancellationToken);

            var actualProcessingResult = context.InvoiceProcessingResults.FirstOrDefault();

            Assert.IsNotNull(actualProcessingResult);
            Assert.AreNotEqual(0, actualProcessingResult.Id);
            Assert.AreNotEqual(default, actualProcessingResult.CreatedDate);
            Assert.AreNotEqual(default, actualProcessingResult.ModifiedDate);
        }

        [TestMethod]
        [DataRow(100, 12, 10)]
        [DataRow(1, 1, 5)]
        public async Task GetAsync_WhenInvoiceProcessingResultExists_ShouldReturnInvoiceProcessingResult(int invoiceProcessingResultId, int invoiceId, int invoiceLinesCount)
        {
            var expectedProcessingResult = CreateInvoiceProcessingResult(invoiceProcessingResultId);
            var invoice = CreateInvoice(invoiceId);
            expectedProcessingResult.InvoiceId = invoice.Id;

            var expectedInvoiceLines = Enumerable
                .Range(invoiceId, invoiceLinesCount)
                .Select(index => CreateInvoiceLine(index, invoice.Id))
                .ToList();

            context.Invoices.Add(invoice);
            context.InvoiceLines.AddRange(expectedInvoiceLines);
            context.InvoiceProcessingResults.Add(expectedProcessingResult);
            context.SaveChanges();

            var actualInvoiceProcessingResult = await target.GetAsync(invoiceProcessingResultId, cancellationToken);

            Assert.IsNotNull(actualInvoiceProcessingResult);
            Assert.IsNotNull(actualInvoiceProcessingResult.Invoice);
            Assert.IsNotNull(actualInvoiceProcessingResult.Invoice.InvoiceLines);
            Assert.AreEqual(invoiceLinesCount, actualInvoiceProcessingResult.Invoice.InvoiceLines.Count);
        }

        [TestMethod]
        [DataRow(100, 12, 3)]
        [DataRow(1, 1, 19)]
        public async Task GetAsync_WhenInvoiceProcessingResultNotExists_ShouldReturnNull(int invoiceProcessingResultId, int invoiceId, int invoiceLinesCount)
        {
            var expectedProcessingResult = CreateInvoiceProcessingResult(invoiceProcessingResultId);
            var invoice = CreateInvoice(invoiceId);
            expectedProcessingResult.InvoiceId = invoice.Id;

            var expectedInvoiceLines = Enumerable
                .Range(invoiceId, invoiceLinesCount)
                .Select(index => CreateInvoiceLine(index, invoice.Id))
                .ToList();

            context.Invoices.Add(invoice);
            context.InvoiceLines.AddRange(expectedInvoiceLines);
            context.InvoiceProcessingResults.Add(expectedProcessingResult);
            context.SaveChanges();

            var actualInvoiceProcessingResult = await target.GetAsync(notExistingInvoiceProcessingResultId, cancellationToken);

            Assert.IsNull(actualInvoiceProcessingResult);
        }

        [TestMethod]
        [DataRow(10, 12, 10)]
        [DataRow(1, 17, 4)]
        public async Task GetByInvoiceIdAsync_WhenInvoiceProcessingResultCollectionIsNotEmpty_ShouldReturnAll(int count, int invoiceId, int invoiceLinesCount)
        {
            var invoice = CreateInvoice(invoiceId);

            var expectedProcessingResults = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoiceProcessingResult(index, invoiceId))
                .ToList();

            var expectedInvoiceLines = Enumerable
                .Range(invoiceId, invoiceLinesCount)
                .Select(index => CreateInvoiceLine(index, invoice.Id))
                .ToList();

            context.InvoiceLines.AddRange(expectedInvoiceLines);
            context.InvoiceProcessingResults.AddRange(expectedProcessingResults);
            context.Invoices.Add(invoice);
            context.SaveChanges();

            var actualInvoiceProcessingResults = await target.GetByInvoiceIdAsync(invoiceId, cancellationToken);

            Assert.IsNotNull(actualInvoiceProcessingResults);
            Assert.AreEqual(expectedProcessingResults.Count, actualInvoiceProcessingResults.Count);

            foreach (var actualInvoiceProcessingResult in actualInvoiceProcessingResults)
            {
                Assert.IsNotNull(actualInvoiceProcessingResult.Invoice);
                Assert.IsNotNull(actualInvoiceProcessingResult.Invoice.InvoiceLines);
                Assert.AreEqual(invoiceLinesCount, actualInvoiceProcessingResult.Invoice.InvoiceLines.Count);
            }
        }

        [TestMethod]
        [DataRow(10, 12, 12)]
        [DataRow(1, 17, 15)]
        public async Task GetByInvoiceIdAsync_WhenInvoiceProcessingResultCollectionIsEmpty_ShouldReturnEmptyList(int count, int invoiceId, int invoiceLinesCount)
        {
            var invoice = CreateInvoice(invoiceId);

            var expectedProcessingResults = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoiceProcessingResult(index, invoiceId))
                .ToList();

            var expectedInvoiceLines = Enumerable
                .Range(invoiceId, invoiceLinesCount)
                .Select(index => CreateInvoiceLine(index, invoice.Id))
                .ToList();

            context.InvoiceLines.AddRange(expectedInvoiceLines);
            context.InvoiceProcessingResults.AddRange(expectedProcessingResults);
            context.Invoices.Add(invoice);
            context.SaveChanges();

            var incorrectInvoiceId = invoiceId + 1;
            var actualInvoiceProcessingResults = await target.GetByInvoiceIdAsync(incorrectInvoiceId, cancellationToken);

            Assert.IsNotNull(actualInvoiceProcessingResults);
            Assert.AreEqual(actualInvoiceProcessingResults.Count, 0);
        }

        [TestMethod]
        [DataRow(10, 12)]
        [DataRow(1, 17)]
        public async Task GetLastByInvoiceIdAsync_WhenInvoiceProcessingResultCollectionIsNotEmpty_ShouldReturnInvoiceProcessingResult(int count, int invoiceId)
        {
            var invoice = CreateInvoice(invoiceId);

            var expectedProcessingResults = Enumerable
                .Range(invoiceId, count)
                .Select(index => CreateInvoiceProcessingResult(index, invoiceId))
                .ToList();

            context.InvoiceProcessingResults.AddRange(expectedProcessingResults);
            context.Invoices.Add(invoice);
            context.SaveChanges();

            var actualInvoiceProcessingResult = await target.GetLastByInvoiceIdAsync(invoiceId, cancellationToken);

            Assert.IsNotNull(actualInvoiceProcessingResult);
            Assert.IsNotNull(actualInvoiceProcessingResult.Invoice);

            var oldResults = expectedProcessingResults
                .Where(processingResult => processingResult.Id != actualInvoiceProcessingResult.Id);
            Assert.IsTrue(oldResults.All(oldResult => oldResult.CreatedDate < actualInvoiceProcessingResult.CreatedDate));
        }

        [TestMethod]
        public async Task UpdateDataAnnotationFileIdAsync_WhenWhenInvoiceIdIsInvalid_ShouldThrowArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateDataAnnotationFileIdAsync(0, "some.file", cancellationToken));
        }

        [TestMethod]
        public async Task UpdateDataAnnotationFileIdAsync_WhenWhenAnnotationFileIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.UpdateDataAnnotationFileIdAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task UpdateDataAnnotationFileIdAsync_WhenValidArguments_ShouldUpdateProcessingResultAndModifiedDate(int invoiceId)
        {
            var invoice = CreateInvoice(invoiceId);
            var storedProcessingResult = CreateInvoiceProcessingResult(1, invoiceId);

            context.InvoiceProcessingResults.Add(storedProcessingResult);
            context.Invoices.Add(invoice);
            context.SaveChanges();
            var newAnnotationFileName = "NewFileName.txt";
            var processingResultId = storedProcessingResult.Id;

            await target.UpdateDataAnnotationFileIdAsync(processingResultId, newAnnotationFileName, cancellationToken);

            var updatedProcessingResult = context.InvoiceProcessingResults.FirstOrDefault(processingResult => processingResult.Id == processingResultId);
            var processingResultToCheck = storedProcessingResult;
            processingResultToCheck.DataAnnotationFileId = newAnnotationFileName;
            processingResultToCheck.ModifiedDate = updatedProcessingResult.ModifiedDate;

            Assert.IsNotNull(updatedProcessingResult);
            AssertInvoiceProcessingResultsAreEqual(processingResultToCheck, updatedProcessingResult);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("   ")]
        public async Task GetVendorNameByTemplateIdAsync_WhenTemplateIdIsInvalid_ShouldThrowArgumentException(string templateId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.GetVendorNameByTemplateIdAsync(templateId, cancellationToken));
        }

        [DataRow("templ12")]
        [TestMethod]
        public async Task GetVendorNameByTemplateIdAsync_WhenValidArguments_ShouldReturnName(string templateId)
        {
            var invoice = CreateInvoice(1, InvoiceStatus.Completed);
            var vendorName = "SomeVendorName";
            var processingResult = CreateInvoiceProcessingResult(11, 1, templateId);
            var invoiceField = CreateInvoiceField(21, 1, 1, vendorName );
            context.InvoiceProcessingResults.Add(processingResult);
            context.InvoiceFields.Add(invoiceField);
            context.Invoices.Add(invoice);

            context.SaveChanges();

            var result = await target.GetVendorNameByTemplateIdAsync(templateId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(vendorName, result);
        }



        private InvoiceProcessingResult CreateInvoiceProcessingResult(int id, int? invoiceId = null, string templateId = null)
        {
            var random = new Random(id);

            if (templateId == null)
            {
                templateId = random.Next().ToString();
            }

            return new InvoiceProcessingResult
            {
                Id = id,
                InvoiceId = invoiceId.HasValue ? invoiceId.Value : random.Next(),
                ProcessingType = InvoiceProcessingType.OCR,
                DataAnnotationFileId = random.Next().ToString(),
                InitialDataAnnotationFileId = "Initial.json",
                TemplateId = templateId,
                CreatedDate = DateTime.UtcNow
            };
        }
        private InvoiceField CreateInvoiceField(int id, int invoiceId, int fieldId, string value)
        {
            return new InvoiceField
            {
                Id = id,
                InvoiceId = invoiceId,
                FieldId = fieldId,
                Value = value
            };
        }

        private Invoice CreateInvoice(int id, InvoiceStatus invoiceStatus = InvoiceStatus.NotStarted)
        {
            var random = new Random(id);

            return new Invoice
            {
                Id = id,
                Name = $"Name{id}",
                FileId = $"File{id}",
                FileName = $"FileName{id}",
                FileSourceType = FileSourceType.API,
                Status = invoiceStatus
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

        private void AssertInvoiceProcessingResultsAreEqual(InvoiceProcessingResult expected, InvoiceProcessingResult actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.InvoiceId, actual.InvoiceId);
            Assert.AreEqual(expected.ProcessingType, actual.ProcessingType);
            Assert.AreEqual(expected.TemplateId, actual.TemplateId);
            Assert.AreEqual(expected.DataAnnotationFileId, actual.DataAnnotationFileId);
            Assert.AreEqual(expected.InitialDataAnnotationFileId, actual.InitialDataAnnotationFileId);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.TrainingFileCount, actual.TrainingFileCount);
            Assert.IsNull(actual.CultureName);
            Assert.IsNull(actual.VendorName);
            // Ensure all properties are tested

            Assert.AreEqual(13, actual.GetType().GetProperties().Length);
        }

        private DatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IInvoiceProcessingResultRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;

        private const int notExistingInvoiceProcessingResultId = 123;
    }
}
