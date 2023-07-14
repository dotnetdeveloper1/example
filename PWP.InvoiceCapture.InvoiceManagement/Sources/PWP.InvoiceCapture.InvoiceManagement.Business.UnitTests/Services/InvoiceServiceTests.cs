using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceRepositoryMock = mockRepository.Create<IInvoiceRepository>();
            documentApiClientMock = mockRepository.Create<IDocumentApiClient>();
            serviceBusPublisherMock = mockRepository.Create<IServiceBusPublisher>();
            target = new InvoiceService(invoiceRepositoryMock.Object, documentApiClientMock.Object, serviceBusPublisherMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceService(null, documentApiClientMock.Object, serviceBusPublisherMock.Object));
        }

        [TestMethod]
        public void Instance_WhenDocumentApiClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceService(invoiceRepositoryMock.Object, null, serviceBusPublisherMock.Object));
        }

        [TestMethod]
        public void Instance_WhenDserviceBusPublisherIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceService(invoiceRepositoryMock.Object, documentApiClientMock.Object, null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceIsNotNull_ShouldSaveInvoice()
        {
            var invoice = new Invoice();

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.CreateAsync(invoice, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.CreateAsync(invoice, cancellationToken);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenInvoiceIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.UpdateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenInvoiceIsNotNull_ShouldSaveInvoice()
        {
            var invoice = new Invoice();

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.UpdateAsync(invoice, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.UpdateAsync(invoice, cancellationToken);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task UpdateStatusAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateStatusAsync(invoiceId, InvoiceStatus.PendingReview, cancellationToken));
        }

        [TestMethod]
        [DataRow(10, InvoiceState.Active)]
        [DataRow(20, InvoiceState.Archived)]
        [DataRow(30, InvoiceState.Deleted)]
        public async Task UpdateStateAsync_WhenArgumentsAreValid_ShouldUpdateInvoiceState(int invoiceId, InvoiceState state)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.UpdateStateAsync(invoiceId, state, cancellationToken))
                .Returns(Task.CompletedTask);
            invoiceRepositoryMock
               .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
               .Returns(Task.FromResult(new Invoice()));

            var result = await target.UpdateStateAsync(invoiceId, state, cancellationToken);
            Assert.AreEqual(OperationResult.Success.Status, result.Status);
        }

        [TestMethod]
        public async Task UpdateStateAsync_WhenInvoiceNotExists_ShouldReturnNotFound()
        {
            invoiceRepositoryMock
               .Setup(invoiceRepository => invoiceRepository.GetAsync(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync((Invoice)null);

            var result = await target.UpdateStateAsync(1, InvoiceState.Archived, cancellationToken);
           
            Assert.AreEqual(OperationResult.NotFound.Status, result.Status);
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
        [DataRow(10, InvoiceStatus.Queued)]
        [DataRow(20, InvoiceStatus.InProgress)]
        [DataRow(30, InvoiceStatus.PendingReview)]
        public async Task UpdateStatusAsync_WhenArgumentsAreValid_ShouldUpdateInvoiceStatus(int invoiceId, InvoiceStatus status)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.UpdateStatusAsync(invoiceId, status, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.UpdateStatusAsync(invoiceId, status, cancellationToken);
        }

        [TestMethod]
        public async Task GetListAsync_WhenInvoicesCollectionExists_ShouldReturnInvoices()
        {
            var expectedInvoices = new List<Invoice>() { new Invoice() };

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedInvoices);

            var actualInvoices = await target.GetListAsync(cancellationToken);

            Assert.AreEqual(expectedInvoices.Count, actualInvoices.Count);
            Assert.AreEqual(1, actualInvoices.Count);
            AssertInvoicesAreEqual(expectedInvoices.Single(), actualInvoices.Single());
        }

        [TestMethod]
        public async Task GetListAsync_WhenRepositoryThrowError_ShouldRethrowException()
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetListAsync(cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetListAsync(cancellationToken));
        }

        [TestMethod]
        public async Task GetPaginatedListAsync_WhenInvoicesCollectionExists_ShouldReturnInvoices()
        {
            var paginatedRequest = new InvoicePaginatedRequest();
            var expectedInvoices = new List<Invoice>() { new Invoice() };
            var expectedResult = new PaginatedResult<Invoice>
            {
                Items = expectedInvoices,
                TotalItemsCount = expectedInvoices.Count
            };

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetPaginatedListAsync(paginatedRequest, cancellationToken))
                .ReturnsAsync(expectedResult);

            var actualPaginatedResult = await target.GetPaginatedListAsync(paginatedRequest, cancellationToken);
            Assert.IsNotNull(actualPaginatedResult);
            Assert.IsNotNull(actualPaginatedResult.Items);
            Assert.AreEqual(1, actualPaginatedResult.TotalItemsCount);

            var actualInvoices = actualPaginatedResult.Items;
            Assert.AreEqual(expectedInvoices.Count, actualInvoices.Count);
            Assert.AreEqual(1, actualInvoices.Count);

            AssertInvoicesAreEqual(expectedInvoices.Single(), actualInvoices.Single());
        }

        [TestMethod]
        public async Task GetPaginatedListAsync_WhenRepositoryThrowError_ShouldRethrowException()
        {
            var paginatedRequest = new InvoicePaginatedRequest();

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetPaginatedListAsync(paginatedRequest, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetPaginatedListAsync(paginatedRequest, cancellationToken));
        }

        [TestMethod]
        public async Task GetPaginatedListAsync_WhenPaginatedRequestIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.GetPaginatedListAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task GetActiveListAsync_WhenRepositoryThrowError_ShouldRethrowException()
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetActiveListAsync(cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetActiveListAsync(cancellationToken));
        }

        [TestMethod]
        public async Task GetActiveListAsync_WhenInvoicesCollectionExists_ShouldReturnInvoices()
        {
            var expectedInvoices = new List<Invoice>() { new Invoice() };

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetActiveListAsync(cancellationToken))
                .ReturnsAsync(expectedInvoices);

            var actualInvoices = await target.GetActiveListAsync(cancellationToken);

            Assert.AreEqual(expectedInvoices.Count, actualInvoices.Count);
            Assert.AreEqual(1, actualInvoices.Count);
            AssertInvoicesAreEqual(expectedInvoices.Single(), actualInvoices.Single());
        }

        [TestMethod]
        public async Task GetActivePaginatedListAsync_WhenInvoicesCollectionExists_ShouldReturnInvoices()
        {
            var paginatedRequest = new InvoicePaginatedRequest();
            var expectedInvoices = new List<Invoice>() { new Invoice() };
            var expectedResult = new PaginatedResult<Invoice>
            {
                Items = expectedInvoices,
                TotalItemsCount = expectedInvoices.Count
            };

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetActivePaginatedListAsync(paginatedRequest, cancellationToken))
                .ReturnsAsync(expectedResult);

            var actualPaginatedResult = await target.GetActivePaginatedListAsync(paginatedRequest, cancellationToken);
            Assert.IsNotNull(actualPaginatedResult);
            Assert.IsNotNull(actualPaginatedResult.Items);
            Assert.AreEqual(1, actualPaginatedResult.TotalItemsCount);

            var actualInvoices = actualPaginatedResult.Items;
            Assert.AreEqual(expectedInvoices.Count, actualInvoices.Count);
            Assert.AreEqual(1, actualInvoices.Count);

            AssertInvoicesAreEqual(expectedInvoices.Single(), actualInvoices.Single());
        }

        [TestMethod]
        public async Task GetActivePaginatedListAsync_WhenRepositoryThrowError_ShouldRethrowException()
        {
            var paginatedRequest = new InvoicePaginatedRequest();

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetActivePaginatedListAsync(paginatedRequest, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetActivePaginatedListAsync(paginatedRequest, cancellationToken));
        }

        [TestMethod]
        public async Task GetActivePaginatedListAsync_WhenPaginatedRequestIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.GetActivePaginatedListAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public async Task GetAsync_WhenDocumentIdIsNullOrWhitespace_ShouldThrowArgumentException(string documentId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.GetAsync(documentId, cancellationToken));
        }

        [TestMethod]
        [DataRow("file1.png")]
        [DataRow("file2.pdf")]
        public async Task GetAsync_WhenInvoiceNotFound_ShouldReturnNull(string documentId)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(documentId, cancellationToken))
                .ReturnsAsync((Invoice)null);

            var actualInvoice = await target.GetAsync(documentId, cancellationToken);

            Assert.IsNull(actualInvoice);
        }

        [TestMethod]
        [DataRow("file1.png")]
        [DataRow("file2.pdf")]
        public async Task GetAsync_WhenInvoiceFound_ShouldReturnInvoice(string documentId)
        {
            var expectedInvoice = new Invoice();

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(documentId, cancellationToken))
                .ReturnsAsync(expectedInvoice);

            var actualInvoice = await target.GetAsync(documentId, cancellationToken);

            AssertInvoicesAreEqual(expectedInvoice, actualInvoice);
        }

        [TestMethod]
        [DataRow("file1.png")]
        [DataRow("file2.pdf")]
        public async Task GetAsync_WhenRepositoryThrowError_ShouldRethrowException(string documentId)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(documentId, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetAsync(documentId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenInvoiceNotFound_ShouldReturnNull(int invoiceId)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
                .ReturnsAsync((Invoice)null);

            var actualInvoice = await target.GetAsync(invoiceId, cancellationToken);

            Assert.IsNull(actualInvoice);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenInvoiceFound_ShouldReturnInvoice(int invoiceId)
        {
            var expectedInvoice = new Invoice();

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
                .ReturnsAsync(expectedInvoice);

            var actualInvoice = await target.GetAsync(invoiceId, cancellationToken);

            Assert.AreEqual(expectedInvoice, actualInvoice);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenRepositoryThrowError_ShouldRethrowException(int invoiceId)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetDocumentFileLinkAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetDocumentFileLinkAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetDocumentFileLinkAsync_WhenRepositoryThrowError_ShouldRethrowException(int invoiceId)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetDocumentFileLinkAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetDocumentFileLinkAsync_WhenInvoiceNotFound_ShouldReturnNull(int invoiceId)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
                .ReturnsAsync((Invoice)null);

            var documentLink = await target.GetDocumentFileLinkAsync(invoiceId, cancellationToken);

            Assert.IsNull(documentLink);
        }

        [TestMethod]
        [DataRow(1, "3e09148f-5545-41db-abd4-e1f8ce7b0329")]
        [DataRow(10, "4cd59de5-2d35-4af4-bbe6-c43a88a4450c")]
        public async Task GetDocumentFileLinkAsync_WhenDocumentApiClientThrowError_ShouldRethrowException(int invoiceId, string fileId)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
                .ReturnsAsync(new Invoice() { FileId = fileId });

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetTemporaryLinkAsync(fileId, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetDocumentFileLinkAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, "3e09148f-5545-41db-abd4-e1f8ce7b0329", "")]
        [DataRow(2, "3e09148f-5545-41db-abd4-e1f8ce7b0329", "             ")]
        [DataRow(2, "3e09148f-5545-41db-abd4-e1f8ce7b0329", "testlink")]
        [DataRow(1, "3e09148f-5545-41db-abd4-e1f8ce7b0329", "http://document.api/3e09148f-5545-41db-abd4-e1f8ce7b0329?token=test")]
        [DataRow(2, "3e09148f-5545-41db-abd4-e1f8ce7b0329", "https://document.api.com/3e09148f-5545-41db-abd4-e1f8ce7b0329.png?token=")]
        public async Task GetDocumentFileLinkAsync_WhenInvoiceDocumentFound_ShouldReturnDocumentApiLink(int invoiceId, string fileId, string expectedLink)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
                .ReturnsAsync(new Invoice() { FileId = fileId });

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetTemporaryLinkAsync(fileId, cancellationToken))
                .ReturnsAsync(new ApiResponse<string> { Data = expectedLink });

            var actualLink = await target.GetDocumentFileLinkAsync(invoiceId, cancellationToken);

            Assert.AreEqual(expectedLink, actualLink);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task RedoAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.RedoAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        public async Task RedoAsync_WhenArgumentsAreValid_ShouldRedo()
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.LockAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.UpdateStatusAsync(invoiceId, InvoiceStatus.PendingReview, cancellationToken))
                .Returns(Task.CompletedTask);
            
            invoiceRepositoryMock
               .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
               .Returns(Task.FromResult(new Invoice() { InvoiceState = InvoiceState.Active, Status = InvoiceStatus.Completed}));

            var result = await target.RedoAsync(invoiceId, cancellationToken);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
        }


        [DataRow(InvoiceState.Deleted)]
        [DataRow(InvoiceState.Locked)]
        [TestMethod]
        public async Task RedoAsync_WhenStateNotValid_ShouldReturnFailedResult(InvoiceState state)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.LockAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceRepositoryMock
               .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
               .Returns(Task.FromResult(new Invoice() { InvoiceState = state, Status = InvoiceStatus.Completed }));

            var result = await target.RedoAsync(invoiceId, cancellationToken);
            
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual("Redo can't be applied to Invoice in state Deleted or Locked.", result.Message);
        }

        [DataRow(InvoiceState.Archived)]
        [TestMethod]
        public async Task RedoAsync_WhenStateArchived_ShouldRedo(InvoiceState state)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.LockAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.UpdateStatusAsync(invoiceId, InvoiceStatus.PendingReview, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.UpdateStateAsync(invoiceId, InvoiceState.Active, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceRepositoryMock
               .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
               .Returns(Task.FromResult(new Invoice() { InvoiceState = state, Status = InvoiceStatus.Completed }));

            var result = await target.RedoAsync(invoiceId, cancellationToken);

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
        }

        [DataRow(InvoiceStatus.Queued)]
        [DataRow(InvoiceStatus.PendingReview)]
        [DataRow(InvoiceStatus.InProgress)]
        [DataRow(InvoiceStatus.NotStarted)]
        [TestMethod]
        public async Task RedoAsync_WhenStatusNotValid_ShouldReturnFailedResult(InvoiceStatus status)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.LockAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceRepositoryMock
               .Setup(invoiceRepository => invoiceRepository.GetAsync(invoiceId, cancellationToken))
               .Returns(Task.FromResult(new Invoice() { InvoiceState = InvoiceState.Active, Status = status }));

            var result = await target.RedoAsync(invoiceId, cancellationToken);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
        }

        [TestMethod]
        public async Task RedoAsync_WhenInvoiceNotExists_ShouldReturnNotFound()
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.LockAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceRepositoryMock
               .Setup(invoiceRepository => invoiceRepository.GetAsync(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync((Invoice)null);

            var result = await target.RedoAsync(invoiceId,  cancellationToken);

            Assert.AreEqual(OperationResultStatus.NotFound, result.Status);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task UpdateValidationMessageAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateValidationMessageAsync(invoiceId, "someMessage", cancellationToken));
        }

        [TestMethod]
        [DataRow(1, "someError")]
        [DataRow(10, "someError2")]
        public async Task UpdateValidationMessageAsync_ShouldUpdate(int invoiceId, string message)
        {
            invoiceRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.UpdateValidationMessageAsync(invoiceId, message, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.UpdateValidationMessageAsync(invoiceId, message, cancellationToken);
        }

        [TestMethod]
        public void GetInvoiceFileSourceTypes_ShouldReturnDictionary()
        {
            var result = target.GetInvoiceFileSourceTypes();

            AssertEnumDictionary<FileSourceType>(result);
        }

        [TestMethod]
        public void GetInvoiceStatuses_ShouldReturnDictionary()
        {
            var result = target.GetInvoiceStatuses();

            AssertEnumDictionary<InvoiceStatus>(result);
        }

        [TestMethod]
        public void GetInvoiceStates_ShouldReturnDictionary()
        {
            var result = target.GetInvoiceStates();

            AssertEnumDictionary<InvoiceState>(result);
        }

        [TestMethod]
        public void GetInvoiceProcessingTypes_ShouldReturnDictionary()
        {
            var result = target.GetInvoiceProcessingTypes();

            AssertEnumDictionary<InvoiceProcessingType>(result);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task PublishInvoiceStatusChangedMessageAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.PublishInvoiceStatusChangedMessageAsync(invoiceId, "some string", cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("  ")]
        public async Task PublishInvoiceStatusChangedMessageAsync_WhenTenantIdIsWrong_ShouldThrowArgumentException(string tenantId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.PublishInvoiceStatusChangedMessageAsync(1, tenantId, cancellationToken));
        }

        [TestMethod]
        [DataRow(12,"13")]
        public async Task PublishInvoiceStatusChangedMessageAsync_ShouldPublish(int invoiceId, string tenantId)
        {
            var actualMessages = new List<InvoiceStatusChangedMessage>();
            serviceBusPublisherMock
                .Setup(serviceBusPublisher => serviceBusPublisher.PublishAsync(Capture.In(actualMessages), cancellationToken))
                .Returns(Task.CompletedTask);

            await target.PublishInvoiceStatusChangedMessageAsync(invoiceId, tenantId, cancellationToken);

            Assert.AreEqual(1, actualMessages.Count);
            Assert.AreEqual(invoiceId, actualMessages[0].InvoiceId);
            Assert.AreEqual(tenantId, actualMessages[0].TenantId);
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

        private void AssertEnumDictionary<T>(Dictionary<string, int> enumValues) where T : IConvertible
        {
            var values = Enum.GetValues(typeof(T)).Cast<int>();
            foreach (var value in values)
            {
                var name = Enum.GetName(typeof(T), value);
                Assert.AreEqual(enumValues[name], value);
            }
        }

        private MockRepository mockRepository;
        private Mock<IInvoiceRepository> invoiceRepositoryMock;
        private Mock<IDocumentApiClient> documentApiClientMock;
        private Mock<IServiceBusPublisher> serviceBusPublisherMock;
        private InvoiceService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int invoiceId = 23;
    }
}
