using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceFieldServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceFieldRepositoryMock = mockRepository.Create<IInvoiceFieldRepository>();
            cancellationToken = CancellationToken.None;
            target = new InvoiceFieldService(invoiceFieldRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceFieldRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceFieldService(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteAsync_WhenInvoiceFieldIdIsZeroOrLessThenZero_ShouldThrowArgumentException(int invoiceFieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(invoiceFieldId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteAsync_WhenInvoiceFieldIdIsCorrectExists_ShouldDeleteInvoiceField(int invoiceFieldId)
        {
            invoiceFieldRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.DeleteAsync(invoiceFieldId, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.DeleteAsync(invoiceFieldId, cancellationToken);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteAsync_WhenInvoiceFieldIdsContainsLessOrEqualZero_ShouldThrowArgumentException(int incorrectId)
        {
            var ids = new List<int> { 1, incorrectId, 3 };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(ids, cancellationToken));
        }

        [TestMethod]
        public async Task DeleteAsync_WhenInvoiceFieldIdsAreCorrectExists_ShouldDeleteInvoiceField()
        {
            var ids = new List<int> { 1, 2, 3 };

            invoiceFieldRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.DeleteAsync(ids, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.DeleteAsync(ids, cancellationToken);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetAsync_WhenInvoiceFieldIdIsLessOrEqualsZero_ShouldReturnInvoiceField(int invoiceFieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(invoiceFieldId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task GetAsync_WhenInvoiceFieldExists_ShouldReturnInvoiceField(int invoiceFieldId)
        {
            var expectedInvoiceField = new InvoiceField() { Id = invoiceFieldId };

            invoiceFieldRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.GetAsync(invoiceFieldId, cancellationToken))
                .ReturnsAsync(expectedInvoiceField);

            var actualInvoiceField = await target.GetAsync(invoiceFieldId, cancellationToken);

            Assert.AreEqual(expectedInvoiceField, actualInvoiceField);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task GetListAsync_WhenInvoiceFieldsCollectionExists_ShouldReturnInvoiceFields(int invoiceId)
        {
            var expectedInvoiceFields = new List<InvoiceField>() { new InvoiceField(), new InvoiceField() };

            invoiceFieldRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync(expectedInvoiceFields);

            var actualInvoiceFields = await target.GetListAsync(invoiceId, cancellationToken);

            Assert.AreEqual(expectedInvoiceFields, actualInvoiceFields);
        }

        [TestMethod]
        public void CreateAsync_WhenInvoiceFieldIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync((InvoiceField)null, cancellationToken));
        }

        [TestMethod]
        public void CreateAsync_WhenInvoiceFieldsAreNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync((List<InvoiceField>)null, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceFieldIsNotNull_ShouldCreateInvoiceField()
        {
            var expectedInvoiceField = new InvoiceField();

            invoiceFieldRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.CreateAsync(expectedInvoiceField, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.CreateAsync(expectedInvoiceField, cancellationToken);
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceFieldsAreNotNull_ShouldCreateInvoiceField()
        {
            var expectedInvoiceFields = new List<InvoiceField>();

            invoiceFieldRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.CreateAsync(expectedInvoiceFields, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.CreateAsync(expectedInvoiceFields, cancellationToken);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenInvoiceFieldIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateAsync_WhenInvoiceFieldIdIsLessOrEqualsZero_ShouldThrowArgumentException(int invoiceFieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateAsync(invoiceFieldId, new InvoiceField(), cancellationToken));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenInvoiceFieldIsNotNull_ShouldUpdateInvoiceField()
        {
            var invoiceField = new InvoiceField() { Id = 1 };

            invoiceFieldRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.UpdateAsync(invoiceField.Id, invoiceField, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.UpdateAsync(invoiceField.Id, invoiceField, cancellationToken);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenInvoiceFieldsAreNotNull_ShouldUpdateInvoiceFields()
        {
            var invoiceFields = new List<InvoiceField> { new InvoiceField() { Id = 1 } };

            invoiceFieldRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.UpdateAsync(invoiceFields, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.UpdateAsync(invoiceFields, cancellationToken);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateAsync_WhenInvoiceFieldsContainsFieldWithLessOrEqualZeroId_ShouldThrowArgumentException(int incorrectId)
        {
            var invoiceFields = new List<InvoiceField> { new InvoiceField() { Id = 1 }, new InvoiceField() { Id = incorrectId } };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateAsync(invoiceFields, cancellationToken));
        }

        private MockRepository mockRepository;
        private Mock<IInvoiceFieldRepository> invoiceFieldRepositoryMock;
        private InvoiceFieldService target;
        private CancellationToken cancellationToken;
    }
}
