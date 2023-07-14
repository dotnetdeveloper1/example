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
    public class InvoiceLineServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceLineRepositoryMock = mockRepository.Create<IInvoiceLineRepository>();
            cancellationToken = CancellationToken.None;
            target = new InvoiceLineService(invoiceLineRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceLineRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceLineService(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteByInvoiceIdAsync_WhenInvoiceIdIsZeroOrLessThenZero_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteByInvoiceIdAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteByInvoiceIdAsync_WhenInvoiceIdIsValid_ShouldDeleteAllInvoiceLinesForInvoice(int invoiceId)
        {
            invoiceLineRepositoryMock
                .Setup(invoiceLineRepository => invoiceLineRepository.DeleteByInvoiceIdAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.DeleteByInvoiceIdAsync(invoiceId, cancellationToken);
        }

        [TestMethod]
        public void CreateAsync_WhenInvoiceLineListIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        public void CreateAsync_WhenInvoiceLineListIsEmpty_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(new List<InvoiceLine>(), cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceLineListIsValid_ShouldCreate()
        {
            var invoiceLines = new List<InvoiceLine>
            {
                new InvoiceLine(),
                new InvoiceLine(),
                new InvoiceLine()
            };

            invoiceLineRepositoryMock
                .Setup(invoiceLineRepository => invoiceLineRepository.CreateAsync(invoiceLines, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.CreateAsync(invoiceLines, cancellationToken);
        }

        private MockRepository mockRepository;
        private Mock<IInvoiceLineRepository> invoiceLineRepositoryMock;
        private InvoiceLineService target;
        private CancellationToken cancellationToken;
    }
}
