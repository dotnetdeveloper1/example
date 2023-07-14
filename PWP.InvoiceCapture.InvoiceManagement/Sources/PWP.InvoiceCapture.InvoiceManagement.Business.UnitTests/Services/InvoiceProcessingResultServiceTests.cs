using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Mappers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceProcessingResultServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceServiceMock = mockRepository.Create<IInvoiceService>();
            invoiceLineServiceMock = mockRepository.Create<IInvoiceLineService>();
            invoiceFieldServiceMock = mockRepository.Create<IInvoiceFieldService>();
            annotationMapperMock = mockRepository.Create<IAnnotationMapper>();
            documentApiClientMock = mockRepository.Create<IDocumentApiClient>();
            serializationServiceMock = mockRepository.Create<ISerializationService>();
            invoiceProcessingResultRepositoryMock = mockRepository.Create<IInvoiceProcessingResultRepository>();
            invoiceTemplateCultureSettingRepositoryMock = mockRepository.Create<IInvoiceTemplateCultureSettingRepository>();
            invoiceProcessingResult = CreateProcessingResult(processingResultId, invoiceId);
            dataAnnotationValidatorMock = mockRepository.Create<IDataAnnotationValidator>();
            publisherMock = mockRepository.Create<IServiceBusPublisher>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            fieldServiceMock = mockRepository.Create<IFieldService>();

            target = new InvoiceProcessingResultService(
                invoiceProcessingResultRepositoryMock.Object,
                invoiceTemplateCultureSettingRepositoryMock.Object,
                invoiceServiceMock.Object,
                invoiceLineServiceMock.Object,
                invoiceFieldServiceMock.Object,
                annotationMapperMock.Object,
                documentApiClientMock.Object,
                serializationServiceMock.Object,
                dataAnnotationValidatorMock.Object, 
                publisherMock.Object,
                applicationContextMock.Object,
                fieldServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceProcessingResultRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(null, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, annotationMapperMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, publisherMock.Object, applicationContextMock.Object, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenInoviceServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, null, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, annotationMapperMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, publisherMock.Object, applicationContextMock.Object, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenInoviceLineServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, null, invoiceFieldServiceMock.Object, annotationMapperMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, publisherMock.Object, applicationContextMock.Object, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenInvoiceFieldServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, invoiceLineServiceMock.Object, null, annotationMapperMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, publisherMock.Object, applicationContextMock.Object, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenAnnotationMapperIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, null, documentApiClientMock.Object, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, publisherMock.Object, applicationContextMock.Object, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenDocumentApiClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, annotationMapperMock.Object, null, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, publisherMock.Object, applicationContextMock.Object, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenSerializationServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, annotationMapperMock.Object, documentApiClientMock.Object, null, dataAnnotationValidatorMock.Object, publisherMock.Object, applicationContextMock.Object, fieldServiceMock.Object));
        }

        public void Instance_WhenDataAnnotationValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, annotationMapperMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, null, publisherMock.Object, applicationContextMock.Object, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenServiceBusPublisherIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, annotationMapperMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, null, applicationContextMock.Object, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, annotationMapperMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, publisherMock.Object, null, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenInvoiceTemplateCultureSettingRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, null, invoiceServiceMock.Object, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, annotationMapperMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, publisherMock.Object, applicationContextMock.Object, fieldServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenFieldServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingResultService(invoiceProcessingResultRepositoryMock.Object, invoiceTemplateCultureSettingRepositoryMock.Object, invoiceServiceMock.Object, invoiceLineServiceMock.Object, invoiceFieldServiceMock.Object, annotationMapperMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, dataAnnotationValidatorMock.Object, publisherMock.Object, applicationContextMock.Object, null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceProcessingResultIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task CreateAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            invoiceProcessingResult.InvoiceId = invoiceId;

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.CreateAsync(invoiceProcessingResult, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task CreateAsync_WhenDataAnnotationFileIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string dataAnnotationFileId)
        {
            invoiceProcessingResult.DataAnnotationFileId = dataAnnotationFileId;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(invoiceProcessingResult, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task CreateAsync_WhenTemplateIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string templateId)
        {
            invoiceProcessingResult.TemplateId = templateId;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(invoiceProcessingResult, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceProcessingResultIsValid_ShouldCreateProcessingResultAndUpdateInvoiceStatus()
        {
            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.CreateAsync(invoiceProcessingResult, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.UpdateStatusAsync(invoiceProcessingResult.InvoiceId, InvoiceStatus.PendingReview, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.CreateAsync(invoiceProcessingResult, cancellationToken);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetListAsync_WhenIsLastFalse_ShouldReturnInvoiceProcessingResultCollection(int count)
        {
            var processingResults = Enumerable
                .Range(1, count)
                .Select(index => CreateProcessingResult(index, invoiceId))
                .ToList();

            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetByInvoiceIdAsync(invoiceId, cancellationToken))
                .ReturnsAsync(processingResults);

            SetupInvoiceTemplateCultureSettingsForGet(new InvoiceTemplateCultureSetting() { CultureName = cultureUs});
            SetupGetVendorName();

            await ProcessListTesting(count, processingResults);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetListAsync_WhenIsLastFalseAndNoCulture_ShouldReturnInvoiceProcessingResultCollection(int count)
        {
            var processingResults = Enumerable
                .Range(1, count)
                .Select(index => CreateProcessingResult(index, invoiceId))
                .ToList();

            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetByInvoiceIdAsync(invoiceId, cancellationToken))
                .ReturnsAsync(processingResults);
            SetupGetVendorName();

            SetupInvoiceTemplateCultureSettingsForGet(null);
            applicationContextMock.SetupGet(x => x.Culture).Returns(cultureUs);

            await ProcessListTesting(count, processingResults);
        }

        [TestMethod]
        public async Task GetLatestAsync_WhenProcessingResultInvoiceIdIsCorrect_ShouldReturnInvoiceProcessingResultCollectionWithOneItem()
        {
            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetLastByInvoiceIdAsync(invoiceId, cancellationToken))
                .ReturnsAsync(invoiceProcessingResult);
           
            SetupInvoiceTemplateCultureSettingsForGet(new InvoiceTemplateCultureSetting() { CultureName = cultureUs});
            SetupGetVendorName();

            var stream = CreateDataAnnotationFileStream();

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetDocumentStreamAsync(invoiceProcessingResult.DataAnnotationFileId, cancellationToken))
                .ReturnsAsync(stream);

            using (var streamReader = new StreamReader(CreateDataAnnotationFileStream()))
            {
                var json = await streamReader.ReadToEndAsync();

                serializationServiceMock
                    .Setup(serializationService => serializationService.Deserialize<DataAnnotation>(json))
                    .Returns(new DataAnnotation());
            }

            var actualProcessingResult = await target.GetLatestAsync(invoiceId, cancellationToken);

            Assert.IsNotNull(actualProcessingResult);
            AssertInvoiceProcessingResultsAreEqual(invoiceProcessingResult, actualProcessingResult);
        }

        [TestMethod]
        public async Task GetLatestAsync_WhenProcessingResultInvoiceIdIsCorrectAndNoCultureSettingsRecord_ShouldReturnInvoiceProcessingResultAndContextCulture()
        {
            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetLastByInvoiceIdAsync(invoiceId, cancellationToken))
                .ReturnsAsync(invoiceProcessingResult);

            SetupInvoiceTemplateCultureSettingsForGet(null);
            SetupGetVendorName();

            applicationContextMock.SetupGet(x => x.Culture).Returns(cultureUs);

            var stream = CreateDataAnnotationFileStream();

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetDocumentStreamAsync(invoiceProcessingResult.DataAnnotationFileId, cancellationToken))
                .ReturnsAsync(stream);

            using (var streamReader = new StreamReader(CreateDataAnnotationFileStream()))
            {
                var json = await streamReader.ReadToEndAsync();

                serializationServiceMock
                    .Setup(serializationService => serializationService.Deserialize<DataAnnotation>(json))
                    .Returns(new DataAnnotation());
            }

            var actualProcessingResult = await target.GetLatestAsync(invoiceId, cancellationToken);

            Assert.IsNotNull(actualProcessingResult);
            AssertInvoiceProcessingResultsAreEqual(invoiceProcessingResult, actualProcessingResult);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetListAsync_WhenInvoiceIdIsIncorrect_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
               target.GetListAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetLatestAsync_WhenInvoiceIdIsIncorrect_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
               target.GetLatestAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        public async Task GetAsync_WhenInvoiceProcessingResultIdIsCorrect_ShouldReturnInvoiceProcessingResult()
        {
            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetAsync(processingResultId, cancellationToken))
                .ReturnsAsync(invoiceProcessingResult);
            
            SetupGetVendorName();
            SetupInvoiceTemplateCultureSettingsForGet(new InvoiceTemplateCultureSetting() { CultureName = cultureUs});
            
            var stream = CreateDataAnnotationFileStream();

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetDocumentStreamAsync(invoiceProcessingResult.DataAnnotationFileId, cancellationToken))
                .ReturnsAsync(stream);

            using (var streamReader = new StreamReader(CreateDataAnnotationFileStream()))
            {
                var json = await streamReader.ReadToEndAsync();

                serializationServiceMock
                    .Setup(serializationService => serializationService.Deserialize<DataAnnotation>(json))
                    .Returns(new DataAnnotation());
            }

            var actualProcessingResult = await target.GetAsync(processingResultId, cancellationToken);

            AssertInvoiceProcessingResultsAreEqual(invoiceProcessingResult, actualProcessingResult);
        }

        [TestMethod]
        public async Task GetAsync_WhenInvoiceProcessingResultIdIsCorrectAndNoCultureSettingsRecord_ShouldReturnInvoiceProcessingResultAndContextCulture()
        {
            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetAsync(processingResultId, cancellationToken))
                .ReturnsAsync(invoiceProcessingResult);

            SetupInvoiceTemplateCultureSettingsForGet(null);
            SetupGetVendorName();

            applicationContextMock.SetupGet(x => x.Culture).Returns(cultureUs);
            
            var stream = CreateDataAnnotationFileStream();

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetDocumentStreamAsync(invoiceProcessingResult.DataAnnotationFileId, cancellationToken))
                .ReturnsAsync(stream);

            using (var streamReader = new StreamReader(CreateDataAnnotationFileStream()))
            {
                var json = await streamReader.ReadToEndAsync();

                serializationServiceMock
                    .Setup(serializationService => serializationService.Deserialize<DataAnnotation>(json))
                    .Returns(new DataAnnotation());
            }

            var actualProcessingResult = await target.GetAsync(processingResultId, cancellationToken);

            AssertInvoiceProcessingResultsAreEqual(invoiceProcessingResult, actualProcessingResult);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetAsync_WhenInvoiceProcessingResultIdIsIncorrect_ShouldThrowArgumentException(int invoiceProcessingId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
               target.GetAsync(invoiceProcessingId, cancellationToken));
        }

        [TestMethod]
        public async Task UpdateDataAnnotationAsync_WhenInvoiceProcessingResultIdIsCorrect_ShouldUpdateDataAnnotationFileId()
        {
            var invoice = new Invoice { Status = InvoiceStatus.PendingReview };
            var invoiceLines = new List<InvoiceLine>() { new InvoiceLine() };

            invoiceProcessingResult.Invoice = invoice;

            var dataAnnotation =  new DataAnnotation()
            {
                InvoiceAnnotations = annotations,
                InvoiceLineAnnotations = lineAnnotations
            };

            var updatedAnnotation = new UpdatedDataAnnotation()
            {
                DataAnnotation = dataAnnotation
            };

            var apiResponse = new ApiResponse<UploadDocumentResponse>();
            apiResponse.Data = new UploadDocumentResponse() { FileId = uploadedFileName };

            SetupLockMocks();
            SetupInvoiceProcessingResultRepositoryMockForUpdate();
            SetupAnnotationMapperMockForUpdate(invoiceLines);
            SetupInvoiceLineServiceMockForUpdate(invoiceLines);
            SetupDataAnnotationValidatorMockForUpdate(dataAnnotation);
            SetupSerializationServiceMockForUpdate();
            SetupDocumentApiClientMockForUpdate(apiResponse);
            applicationContextMock.SetupGet(x => x.Culture).Returns(cultureUs);
            SetupValidationMessageUpdate(invoiceId, null);

            var actualResult = await target.UpdateDataAnnotationAsync(processingResultId, updatedAnnotation, CancellationToken.None);

            Assert.IsNotNull(actualResult);
            Assert.IsTrue(actualResult.IsSuccessful);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-100)]

        public async Task ValidateCreatedInvoiceAsync_WhenInvoiceIdIsWrong_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.ValidateCreatedInvoiceAsync(invoiceId, cultureGb, cancellationToken));
        }

        [TestMethod]
        public async Task ValidateCreatedInvoiceAsync_WhenAnnotationNotValid_ShouldUpdateErrorMessage()
        {
            var dataAnnotation = await CreateValidateCreatedInvoiceMocks();

            dataAnnotationValidatorMock
                .Setup(dataAnnotationValidator => dataAnnotationValidator.Validate(dataAnnotation, new CultureInfo(cultureGb), fields))
                .Returns(ValidationResult.Failed(validationErrorMessage));

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.UpdateValidationMessageAsync(invoiceId, validationErrorMessage, cancellationToken))
                .Returns(Task.CompletedTask);

            fieldServiceMock
                .Setup(fieldService => fieldService.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            await target.ValidateCreatedInvoiceAsync(invoiceId, cultureGb, cancellationToken);
        }

        [TestMethod]
        public async Task ValidateCreatedInvoiceAsync_WhenAnnotationIsValid_ShouldNotUpdateErrorMessage()
        {
            var dataAnnotation = await CreateValidateCreatedInvoiceMocks();

            dataAnnotationValidatorMock
                .Setup(dataAnnotationValidator => dataAnnotationValidator.Validate(dataAnnotation, new CultureInfo(cultureGb), fields))
                .Returns(ValidationResult.Ok);

            fieldServiceMock
                .Setup(fieldService => fieldService.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            await target.ValidateCreatedInvoiceAsync(invoiceId, cultureAu, cancellationToken);
        }

        private void SetupValidationMessageUpdate(int invoiceId, string message)
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.UpdateValidationMessageAsync(invoiceId, message, cancellationToken))
                .Returns(Task.CompletedTask);
        }

        [TestMethod]
        public async Task UpdateDataAnnotationAsync_WhenInvoiceLinesNotPresent_ShouldRemoveExistingLines()
        {
            var invoice = new Invoice { Status = InvoiceStatus.PendingReview };

            invoiceProcessingResult.Invoice = invoice;

            var dataAnnotation = new DataAnnotation() { InvoiceAnnotations = annotations };
            var updatedAnnotation = new UpdatedDataAnnotation() { DataAnnotation = dataAnnotation };
            var apiResponse = new ApiResponse<UploadDocumentResponse>();
            apiResponse.Data = new UploadDocumentResponse() { FileId = uploadedFileName };

            SetupLockMocks();
            SetupInvoiceProcessingResultRepositoryMockForUpdate();
            SetupAnnotationMapperMockForUpdate(null);
            SetupInvoiceLineServiceMockForUpdate(null);
            SetupDataAnnotationValidatorMockForUpdate(dataAnnotation);
            SetupSerializationServiceMockForUpdate();
            SetupDocumentApiClientMockForUpdate(apiResponse);
            applicationContextMock.SetupGet(x => x.Culture).Returns(cultureUs);
            SetupValidationMessageUpdate(invoiceId, null);

            var actualResult = await target.UpdateDataAnnotationAsync(processingResultId, updatedAnnotation, CancellationToken.None);

            Assert.IsNotNull(actualResult);
            Assert.IsTrue(actualResult.IsSuccessful);
        }

        [DataRow(InvoiceStatus.Completed)]
        [DataRow(InvoiceStatus.InProgress)]
        [DataRow(InvoiceStatus.NotStarted)]
        [DataRow(InvoiceStatus.Queued)]
        [TestMethod]
        public async Task UpdateDataAnnotationAsync_WhenInvoiceStatusIsNotPendingReview_ShouldReturnFailed(InvoiceStatus invoiceStatus)
        {
            invoiceProcessingResult = new InvoiceProcessingResult
            {
                Invoice = new Invoice()
                {
                    Status = invoiceStatus
                }
            };

            SetupLockMocks();

            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetAsync(processingResultId, cancellationToken))
                .ReturnsAsync(invoiceProcessingResult);

            var updatedAnnotation = new UpdatedDataAnnotation() 
            {
                DataAnnotation = new DataAnnotation() 
                { 
                    InvoiceAnnotations = annotations
                }
            };

            var actualResult = await target.UpdateDataAnnotationAsync(processingResultId, updatedAnnotation, CancellationToken.None);

            Assert.IsNotNull(actualResult);
            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, actualResult.Status);
            Assert.AreEqual("Invoice is not in status PendingReview.", actualResult.Message);
        }

        
        [TestMethod]
        public async Task UpdateDataAnnotationAsync_WhenProcessingResultNotExists_ShouldReturnNotFound()
        {
            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetInvoiceIdAsync(processingResultId, cancellationToken))
                .ReturnsAsync((int?)null);
            
            var dataAnnotation = new DataAnnotation() { InvoiceAnnotations = annotations };
            var updatedAnnotation = new UpdatedDataAnnotation() { DataAnnotation = dataAnnotation };
            var actualResult = await target.UpdateDataAnnotationAsync(processingResultId, updatedAnnotation, CancellationToken.None);

            Assert.IsNotNull(actualResult);
            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.NotFound, actualResult.Status);
            Assert.AreEqual("Processing result with id = 1 was not found.", actualResult.Message);
        }

        [TestMethod]
        public async Task CompleteAsync_ShouldComplete()
        {
            var actualMessages = new List<InvoiceCompletedMessage>();
            var invoiceLines = new List<InvoiceLine>() { new InvoiceLine() };
            var invoice = new Invoice { Id = invoiceId, Status = InvoiceStatus.PendingReview, FileId = "InvoiceFileId" };

            invoiceProcessingResult.Id = processingResultId;
            invoiceProcessingResult.Invoice = invoice;
            invoiceProcessingResult.InvoiceId = invoiceId;
            invoiceProcessingResult.DataAnnotationFileId = "someguid";
            invoiceProcessingResult.TemplateId = templateId;

            var dataAnnotation = new DataAnnotation()
            {
                InvoiceAnnotations = annotations,
                InvoiceLineAnnotations = lineAnnotations
            };
            var updatedAnnotation = new UpdatedDataAnnotation() { DataAnnotation = dataAnnotation};

            var apiResponse = new ApiResponse<UploadDocumentResponse>();
            apiResponse.Data = new UploadDocumentResponse() { FileId = uploadedFileName };

            SetupLockMocks();
            SetupInvoiceProcessingResultRepositoryMockForUpdate();
            SetupAnnotationMapperMockForUpdate(invoiceLines);
            SetupInvoiceLineServiceMockForUpdate(invoiceLines);
            SetupDataAnnotationValidatorMockForUpdate(dataAnnotation);
            SetupSerializationServiceMockForUpdate();
            SetupDocumentApiClientMockForUpdate(apiResponse);
            SetupValidationMessageUpdate(invoiceId, null);

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.UpdateStatusAsync(invoiceId, InvoiceStatus.Completed, cancellationToken))
                .Returns(Task.CompletedTask);
            invoiceServiceMock
               .Setup(invoiceService => invoiceService.UpdateStateAsync(invoiceId,InvoiceState.Locked, cancellationToken))
                .ReturnsAsync(OperationResult.Success);

            publisherMock
               .Setup(publisher => publisher.PublishAsync(Capture.In(actualMessages), cancellationToken))
               .Returns(Task.CompletedTask);
            applicationContextMock
                .Setup(applicationContext => applicationContext.TenantId)
                .Returns(tenantId);
            applicationContextMock.SetupGet(x => x.Culture).Returns(cultureUs);

            await target.CompleteAsync(processingResultId, updatedAnnotation, cancellationToken);

            Assert.AreEqual(1, actualMessages.Count);
            Assert.AreEqual(invoiceId, actualMessages[0].InvoiceId);
            Assert.AreEqual(invoiceProcessingResult.TemplateId, actualMessages[0].TemplateId);
            Assert.AreEqual(tenantId, actualMessages[0].TenantId);
            Assert.AreEqual(invoice.FileId, actualMessages[0].InvoiceFileId);
            Assert.AreEqual(invoiceProcessingResult.DataAnnotationFileId, actualMessages[0].DataAnnotationFileId);
        }

        private async Task<DataAnnotation> CreateValidateCreatedInvoiceMocks()
        {
            var dataAnnotation = new DataAnnotation();

            invoiceProcessingResultRepositoryMock
                 .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetLastByInvoiceIdAsync(invoiceId, cancellationToken))
                 .ReturnsAsync(new InvoiceProcessingResult() { TemplateId = templateId, DataAnnotationFileId = uploadedFileName });
            invoiceTemplateCultureSettingRepositoryMock
                 .Setup(invoiceTemplateCultureSettingRepository => invoiceTemplateCultureSettingRepository.GetByTemplateIdAsync(templateId, cancellationToken))
                 .ReturnsAsync(new InvoiceTemplateCultureSetting() { CultureName = cultureGb });

            var stream = CreateDataAnnotationFileStream();

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetDocumentStreamAsync(uploadedFileName, cancellationToken))
                .ReturnsAsync(stream);

            using (var streamReader = new StreamReader(CreateDataAnnotationFileStream()))
            {
                var json = await streamReader.ReadToEndAsync();

                serializationServiceMock
                    .Setup(serializationService => serializationService.Deserialize<DataAnnotation>(json))
                    .Returns(dataAnnotation);
            }

            return dataAnnotation;
        }

        private void SetupInvoiceProcessingResultRepositoryMockForUpdate()
        {
            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetAsync(processingResultId, cancellationToken))
                .ReturnsAsync(invoiceProcessingResult);

            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.UpdateDataAnnotationFileIdAsync(processingResultId, uploadedFileName, cancellationToken))
                .Returns(Task.CompletedTask);
        }

        private void SetupAnnotationMapperMockForUpdate(List<InvoiceLine> invoiceLines, string cultureName = null)
        {
            invoiceFieldServiceMock
                .Setup(invoiceFieldService => invoiceFieldService.GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync(invoiceFields);

            annotationMapperMock
                .Setup(mapper => mapper.ToInvoiceField(invoiceId, It.IsAny<Annotation>(), It.IsAny<List<Field>>(), cultureInfo))
                .Returns(newInvoiceFields[0]);

            invoiceFieldServiceMock
                .Setup(invoiceFieldService => invoiceFieldService.CreateAsync(newInvoiceFields, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceFieldServiceMock
                .Setup(invoiceFieldService => invoiceFieldService.UpdateAsync(It.IsAny<List<InvoiceField>>(), cancellationToken))
                .Returns(Task.CompletedTask);

            fieldServiceMock
                .Setup(fieldService => fieldService.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            if (invoiceLines != null)
            {
                var currentCultureInfo = cultureName == null ? cultureInfo : new CultureInfo(cultureName);
                annotationMapperMock
                   .Setup(mapper => mapper.ToInvoiceLine(lineAnnotations.First(), invoiceProcessingResult.InvoiceId, currentCultureInfo))
                   .Returns(invoiceLines.First());
            }
        }

        private void SetupInvoiceLineServiceMockForUpdate(List<InvoiceLine> invoiceLines)
        {
            invoiceLineServiceMock
                .Setup(invoiceLineService => invoiceLineService.DeleteByInvoiceIdAsync(invoiceProcessingResult.InvoiceId, cancellationToken))
                .Returns(Task.CompletedTask);

            if (invoiceLines != null)
            {
                invoiceLineServiceMock
                    .Setup(invoiceLineService => invoiceLineService.CreateAsync(invoiceLines, cancellationToken))
                    .Returns(Task.CompletedTask);
            }
        }

        private UpdatedDataAnnotation CreateUpdatedAnnotations(string culture = null)
        {
            var dataAnnotation = new DataAnnotation()
            {
                InvoiceAnnotations = annotations,
                InvoiceLineAnnotations = lineAnnotations
            };

            var updatedAnnotation = new UpdatedDataAnnotation()
            {
                DataAnnotation = dataAnnotation,
                CultureName = culture
            };
            return updatedAnnotation;
        }

        private void SetupDataAnnotationValidatorMockForUpdate(DataAnnotation dataAnnotation, string cultureName = null)
        {
            var currentCultureInfo = cultureName == null ? cultureInfo : new CultureInfo(cultureName);
            dataAnnotationValidatorMock
                .Setup(requiredField => requiredField.Validate(dataAnnotation, currentCultureInfo, fields))
                .Returns(ValidationResult.Ok);
        }

        private void SetupSerializationServiceMockForUpdate()
        {
            serializationServiceMock
                .Setup(serializationService => serializationService.Serialize(It.IsAny<DataAnnotation>()))
                .Returns(string.Empty);
        }

        private void SetupInvoiceServiceMockForUpdate(Invoice invoice)
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.UpdateAsync(invoice, cancellationToken))
                .Returns(Task.CompletedTask);
        }

        private void SetupDocumentApiClientMockForUpdate(ApiResponse<UploadDocumentResponse> apiResponse)
        {
            documentApiClientMock
               .Setup(documentApiClient => documentApiClient.UploadDocumentAsync(It.IsAny<Stream>(), uploadedFileName, cancellationToken))
               .ReturnsAsync(apiResponse);
        }


        private void SetupInvoiceTemplateCultureSettingsForGet(InvoiceTemplateCultureSetting invoiceTemplateCultureSetting)
        {
            invoiceTemplateCultureSettingRepositoryMock
                 .Setup(invoiceTemplateCultureSettingRepository => invoiceTemplateCultureSettingRepository.GetByTemplateIdAsync(templateId, cancellationToken))
                 .ReturnsAsync(invoiceTemplateCultureSetting);
        }

        private void SetupGetVendorName()
        {
            invoiceProcessingResultRepositoryMock
                 .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetVendorNameByTemplateIdAsync(templateId, cancellationToken))
                 .ReturnsAsync(vendorName);
        }

        private void SetupLockMocks() 
        {
            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.GetInvoiceIdAsync(processingResultId, cancellationToken))
                .ReturnsAsync(invoiceId);

            invoiceProcessingResultRepositoryMock
                .Setup(invoiceProcessingResultRepository => invoiceProcessingResultRepository.LockAsync(processingResultId, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.LockAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);
        }

        private MemoryStream CreateDataAnnotationFileStream()
        {
            var bytes = File.ReadAllBytes($"Files/{fileName}.json");

            return new MemoryStream(bytes);
        }

        private InvoiceProcessingResult CreateProcessingResult(int id, int invoiceId) 
        {
            return new InvoiceProcessingResult 
            {
                Id = id,
                InvoiceId = invoiceId,
                DataAnnotationFileId =  Guid.NewGuid().ToString(),
                TemplateId = templateId,
                ProcessingType = InvoiceProcessingType.OCR,
                TrainingFileCount = trainingFileCount
            };
        }

        private async Task ProcessListTesting(int count, List<InvoiceProcessingResult> processingResults)
        {
            foreach (var processingResult in processingResults)
            {
                var stream = CreateDataAnnotationFileStream();

                documentApiClientMock
                    .Setup(documentApiClient => documentApiClient.GetDocumentStreamAsync(processingResult.DataAnnotationFileId, cancellationToken))
                    .ReturnsAsync(stream);

                using (var streamReader = new StreamReader(CreateDataAnnotationFileStream()))
                {
                    var json = await streamReader.ReadToEndAsync();

                    serializationServiceMock
                        .Setup(serializationService => serializationService.Deserialize<DataAnnotation>(json))
                        .Returns(new DataAnnotation());
                }
            }

            var actualProcessingResults = await target.GetListAsync(invoiceId, cancellationToken);

            for (var index = 1; index <= count; index++)
            {
                AssertInvoiceProcessingResultsAreEqual(processingResults[index - 1], actualProcessingResults[index - 1]);
            }

            Assert.AreEqual(processingResults.Count, actualProcessingResults.Count);
        }

        private void AssertInvoiceProcessingResultsAreEqual(InvoiceProcessingResult expected, InvoiceProcessingResult actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.DataAnnotationFileId, actual.DataAnnotationFileId);
            Assert.AreEqual(expected.InvoiceId, actual.InvoiceId);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.DataAnnotation, actual.DataAnnotation);
            Assert.AreEqual(expected.InitialDataAnnotationFileId, actual.InitialDataAnnotationFileId);
            Assert.AreEqual(expected.TemplateId, actual.TemplateId);
            Assert.AreEqual(expected.ProcessingType, actual.ProcessingType);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);           
            Assert.IsNotNull(actual.DataAnnotation);
            Assert.IsNotNull(actual.CultureName);
            Assert.AreEqual(expected.TrainingFileCount, actual.TrainingFileCount);
            Assert.AreEqual(vendorName, actual.VendorName);

            // Ensure all properties are tested
            Assert.AreEqual(13, actual.GetType().GetProperties().Length);
        }

        private readonly List<Annotation> annotations = new List<Annotation>()
        {
            new Annotation { FieldType = FieldTypes.VendorName.ToString(), FieldValue = "name" },
            new Annotation { FieldType = FieldTypes.VendorAddress.ToString(), FieldValue = "address" },
            new Annotation { FieldType = FieldTypes.TaxNumber.ToString(), FieldValue = "taxN" },
            new Annotation { FieldType = FieldTypes.VendorPhone.ToString(), FieldValue = "phone" },
            new Annotation { FieldType = FieldTypes.VendorEmail.ToString(), FieldValue = "email" },
            new Annotation { FieldType = FieldTypes.VendorWebsite.ToString(), FieldValue = "website" },
            new Annotation { FieldType = FieldTypes.InvoiceDate.ToString(), FieldValue = "2020-01-01" },
            new Annotation { FieldType = FieldTypes.DueDate.ToString(), FieldValue = "2020-01-02" },
            new Annotation { FieldType = FieldTypes.PoNumber.ToString(), FieldValue = "poN" },
            new Annotation { FieldType = FieldTypes.InvoiceNumber.ToString(), FieldValue = "invN1" },
            new Annotation { FieldType = FieldTypes.TaxAmount.ToString(), FieldValue = "1.2" },
            new Annotation { FieldType = FieldTypes.FreightAmount.ToString(), FieldValue = "1.1" },
            new Annotation { FieldType = FieldTypes.SubTotal.ToString(), FieldValue = "2" },
            new Annotation { FieldType = FieldTypes.Total.ToString(), FieldValue = "4.3" },
            new Annotation { FieldType = (FieldTypes.Total + 1).ToString(), FieldValue = "5.4" }
        };

        private readonly List<InvoiceField> newInvoiceFields = new List<InvoiceField>
        {
            new InvoiceField()
            {
                Id = FieldTypes.Total + 1,
                Value = "Custom Invoice Field"
            }
        };

        private readonly List<InvoiceField> invoiceFields = new List<InvoiceField>()
        {
            new InvoiceField { Id = FieldTypes.VendorName, FieldId = FieldTypes.VendorName, Value = "name" },
            new InvoiceField { Id = FieldTypes.VendorAddress, FieldId = FieldTypes.VendorAddress, Value = "address" },
            new InvoiceField { Id = FieldTypes.TaxNumber, FieldId = FieldTypes.TaxNumber, Value = "taxN" },
            new InvoiceField { Id = FieldTypes.VendorPhone, FieldId = FieldTypes.VendorPhone, Value = "phone" },
            new InvoiceField { Id = FieldTypes.VendorEmail, FieldId = FieldTypes.VendorEmail, Value = "email" },
            new InvoiceField { Id = FieldTypes.VendorWebsite, FieldId = FieldTypes.VendorWebsite, Value = "website" },
            new InvoiceField { Id = FieldTypes.InvoiceDate, FieldId = FieldTypes.InvoiceDate, Value = "2020-01-01" },
            new InvoiceField { Id = FieldTypes.DueDate, FieldId = FieldTypes.DueDate, Value = "2020-01-02" },
            new InvoiceField { Id = FieldTypes.PoNumber, FieldId = FieldTypes.PoNumber, Value = "poN" },
            new InvoiceField { Id = FieldTypes.InvoiceNumber, FieldId = FieldTypes.InvoiceNumber, Value = "invN1" },
            new InvoiceField { Id = FieldTypes.TaxAmount, FieldId = FieldTypes.TaxAmount, Value = "1.2" },
            new InvoiceField { Id = FieldTypes.FreightAmount, FieldId = FieldTypes.FreightAmount, Value = "1.1" },
            new InvoiceField { Id = FieldTypes.SubTotal, FieldId = FieldTypes.SubTotal, Value = "2" },
            new InvoiceField { Id = FieldTypes.Total, FieldId = FieldTypes.Total, Value = "4.3" }
        };

        private readonly List<LineAnnotation> lineAnnotations = new List<LineAnnotation>()
        {
            new LineAnnotation
            {
                OrderNumber = 1,
                LineItemAnnotations = new List<Annotation>
                {
                    new Annotation { FieldType = InvoiceLineFieldTypes.Description, FieldValue = description },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Number, FieldValue = number },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Price, FieldValue = price.ToString() },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Quantity, FieldValue = quantity.ToString() },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Total, FieldValue = total.ToString() }
                }
            }
        };

        private MockRepository mockRepository;
        private Mock<IInvoiceService> invoiceServiceMock;
        private Mock<IInvoiceLineService> invoiceLineServiceMock;
        private Mock<IInvoiceFieldService> invoiceFieldServiceMock;
        private Mock<IAnnotationMapper> annotationMapperMock;
        private Mock<IDocumentApiClient> documentApiClientMock;
        private Mock<ISerializationService> serializationServiceMock;
        private Mock<IInvoiceProcessingResultRepository> invoiceProcessingResultRepositoryMock;
        private Mock<IInvoiceTemplateCultureSettingRepository> invoiceTemplateCultureSettingRepositoryMock;
        private Mock<IDataAnnotationValidator> dataAnnotationValidatorMock;
        private Mock<IServiceBusPublisher> publisherMock;
        private Mock<IApplicationContext> applicationContextMock;
        private Mock<IFieldService> fieldServiceMock;
        private InvoiceProcessingResult invoiceProcessingResult;
        private InvoiceProcessingResultService target;
        
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private readonly CultureInfo cultureInfo = new CultureInfo(cultureUs);

        private readonly List<Field> fields = new List<Field>() 
        {
            new Field() {  },
            new Field() {  },
        };

        private const int processingResultId = 1;
        private const int invoiceId = 10;
        private const string tenantId = "11";
        private const string fileName = "DataAnnotation";
        private const string validationErrorMessage = "somewrong validation";
        private const string uploadedFileName = "dataAnnotation.json";
        private const string templateId = "2cc37da3-1d13-2af2-bbe4-c21a66a2230b";
        private const string description = "Description";
        private const string number = "Number";
        private const string vendorName = "SomeVendorName";
        private const decimal price = 10.1M;
        private const int quantity = 10;
        private const int total = 15;
        private const int trainingFileCount = 15;
        private const string cultureUs = "en-Us";
        private const string cultureAu = "en-Au"; 
        private const string cultureGb = "en-GB"; 
    }
}
