using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.API.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TemplateManagementServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            documentApiClientMock = mockRepository.Create<IDocumentApiClient>();
            serializationServiceMock = mockRepository.Create<ISerializationService>();
            invoiceTemplateRepositoryMock = mockRepository.Create<IInvoiceTemplateRepository>();
            formRecognizerTrainingDocumentFactoryMock = mockRepository.Create<IFormRecognizerTrainingDocumentFactory>();
            trainingBlobRepositoryMock = mockRepository.Create<ITrainingBlobRepository>();
            formRecognizerClientMock = mockRepository.Create<IFormRecognizerClient>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            fileNameProviderMock = mockRepository.Create<IFileNameProvider>();

            target = new TemplateManagementService(
                documentApiClientMock.Object, 
                serializationServiceMock.Object, 
                invoiceTemplateRepositoryMock.Object, 
                formRecognizerTrainingDocumentFactoryMock.Object,
                trainingBlobRepositoryMock.Object,
                formRecognizerClientMock.Object,
                telemetryClientMock.Object,
                fileNameProviderMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public void GetTemplateTrainingsCountAsync_WhenValueIsLessOrEqualsZero_ShouldThrowArgumentException(int templateId)
        {
            Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetTemplateTrainingsCountAsync(templateId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 12)]
        [DataRow(123, 1111)]
        public async Task GetTemplateTrainingsCountAsync_WhenValueIsMoreThenZero_ShouldReturnCount(int templateId, int count)
        {
            var template = new InvoiceTemplate() { Id = templateId, TrainingFileCount = count };

            invoiceTemplateRepositoryMock
                .Setup(invoiceTemplateRepository => invoiceTemplateRepository.GetByIdAsync(templateId, cancellationToken))
                .ReturnsAsync(template);

            var trainingsCount = await target.GetTemplateTrainingsCountAsync(templateId, cancellationToken);

            Assert.AreEqual(count, trainingsCount);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public void GetTemplateAsync_WhenValueIsLessOrEqualsZero_ShouldThrowArgumentException(int templateId)
        {
            Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetTemplateAsync(templateId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(123)]
        public async Task GetTemplateTrainingsCountAsync_WhenValueIsMoreThenZero_ShouldReturnCount(int templateId)
        {
            var template = new InvoiceTemplate() { Id = templateId };

            invoiceTemplateRepositoryMock
                .Setup(invoiceTemplateRepository => invoiceTemplateRepository.GetByIdAsync(templateId, cancellationToken))
                .ReturnsAsync(template);

            var invoiceTemplate = await target.GetTemplateAsync(templateId, cancellationToken);

            Assert.AreEqual(template.Id, invoiceTemplate.Id);
        }

        private Mock<IDocumentApiClient> documentApiClientMock;
        private Mock<ISerializationService> serializationServiceMock;
        private Mock<IInvoiceTemplateRepository> invoiceTemplateRepositoryMock;
        private Mock<IFormRecognizerTrainingDocumentFactory> formRecognizerTrainingDocumentFactoryMock;
        private Mock<ITrainingBlobRepository> trainingBlobRepositoryMock;
        private Mock<IFormRecognizerClient> formRecognizerClientMock;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IFileNameProvider> fileNameProviderMock;

        private TemplateManagementService target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
