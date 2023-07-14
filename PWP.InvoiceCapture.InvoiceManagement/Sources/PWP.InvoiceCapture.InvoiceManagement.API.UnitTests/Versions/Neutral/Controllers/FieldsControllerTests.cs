using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.API.Versions.Neutral.Controllers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.API.UnitTests.Versions.Neutral.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FieldsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            fieldServiceMock = mockRepository.Create<IFieldService>();
            target = new FieldsController(fieldServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenFieldServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldsController(null));
        }

        [TestMethod]
        public async Task GetFieldListAsync_WhenFieldsCollectionExists_ShouldReturnOkResultWithFieldList()
        {
            var expectedFields = new List<Field> { new Field() };

            fieldServiceMock
                .Setup(fieldService => fieldService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedFields);

            var result = await target.GetFieldsListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<Field>>;

            Assert.IsInstanceOfType(apiResponce.Data, expectedFields.GetType());
            Assert.AreEqual(expectedFields, apiResponce.Data);
        }

        [TestMethod]
        public async Task GetFieldListAsync_WhenFieldsNotFound_ShouldReturnOkResultWithNull()
        {
            fieldServiceMock
                .Setup(fieldService => fieldService.GetListAsync(cancellationToken))
                .ReturnsAsync((List<Field>)null);

            var result = await target.GetFieldsListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<Field>>;

            Assert.IsNull(apiResponce.Data);
        }

        [TestMethod]
        public async Task GetFieldListAsync_WhenFieldsWereFound_ShouldReturnOkResult()
        {
            var expectedFields = new List<Field>() { new Field() };

            fieldServiceMock
                .Setup(fieldService => fieldService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedFields);

            var result = await target.GetFieldsListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<Field>>;

            Assert.IsNotNull(apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetFieldByIdAsync_WhenFieldFound_ShouldReturnOkResultWithField(int fieldId)
        {
            var expectedField = new Field();

            fieldServiceMock
                .Setup(fieldService => fieldService.GetAsync(fieldId, cancellationToken))
                .ReturnsAsync(expectedField);

            var result = await target.GetFieldByIdAsync(fieldId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<Field>;

            Assert.IsNotNull(apiResponce);
            Assert.IsInstanceOfType(apiResponce.Data, expectedField.GetType());
            Assert.AreEqual(expectedField, apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetFieldByIdAsync_WhenFieldNotFound_ShouldReturn404NotFoundResult(int fieldId)
        {
            fieldServiceMock
                .Setup(fieldService => fieldService.GetAsync(fieldId, cancellationToken))
                .ReturnsAsync((Field)null);

            var result = await target.GetFieldByIdAsync(fieldId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Field with id = {fieldId} not found.");
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetFieldByIdAsync_WhenFieldIdParameterOutOfRange_ShouldReturnBadRequestResult(int fieldId)
        {
            var result = await target.GetFieldByIdAsync(fieldId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(fieldId)} parameter has invalid value.");
        }

        [TestMethod]
        public async Task CreateAsync_WhenFieldExists_ShouldReturnOkObjectResult()
        {
            var fieldToSave = new Field() { DisplayName = "DisplayName" };
            var message = $"Field with name = {fieldToSave.DisplayName} was created.";

            fieldServiceMock
                .Setup(fieldService => fieldService.CreateAsync(fieldToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Success, Message = message });

            var result = await target.CreateFieldAsync(fieldToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [TestMethod]
        public async Task CreateAsync_WhenFieldHasWrongTargeFieldType_ShouldReturnBadRequestObjectResult()
        {
            var fieldToSave = new Field() { DisplayName = "DisplayName", TargetFieldType = (TargetFieldType)1000 };
            var message = $"Field contains unknown target field type.";

            fieldServiceMock
                .Setup(fieldService => fieldService.CreateAsync(fieldToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Failed, Message = message });

            var result = await target.CreateFieldAsync(fieldToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateFieldAsync_WhenFieldExists_ShouldReturnOkObjectResult(int fieldId)
        {
            var fieldToSave = new Field() { Id = fieldId };
            var message = $"Field with id = {fieldId} was updated.";

            fieldServiceMock
                .Setup(fieldService => fieldService.UpdateAsync(fieldId, fieldToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Success, Message = message });

            var result = await target.UpdateAsync(fieldId, fieldToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateFieldAsync_WhenFieldHasWrongTargeFieldType_ShouldReturnBadRequestObjectResult(int fieldId)
        {
            var fieldToSave = new Field() { Id = fieldId, TargetFieldType = (TargetFieldType)1000 };
            var message = $"Field contains unknown target field type.";

            fieldServiceMock
                .Setup(fieldService => fieldService.UpdateAsync(fieldId, fieldToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Failed, Message = message });

            var result = await target.UpdateAsync(fieldId, fieldToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateFieldAsync_WhenFieldNotExists_ShouldReturnBadRequestObjectResult(int fieldId)
        {
            var fieldToSave = new Field() { Id = fieldId };
            var message = $"Field with id = {fieldId} not found.";

            fieldServiceMock
                .Setup(fieldService => fieldService.GetAsync(fieldId, cancellationToken))
                .ReturnsAsync(fieldToSave);

            fieldServiceMock
                .Setup(fieldService => fieldService.UpdateAsync(fieldId, fieldToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.NotFound, Message = message });

            var result = await target.UpdateAsync(fieldId, fieldToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-10)]
        public async Task UpdateFieldAsync_WhenFieldIdIsLessOrEqualstoZeroNotFound_ShouldReturnBadRequestObjectResult(int fieldId)
        {
            var fieldToSave = new Field() { Id = fieldId };

            var result = await target.UpdateAsync(fieldId, fieldToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(fieldId)} parameter has invalid value.");
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task DeleteAsync_WhenFieldWasRemoved_ShouldReturnOkObjectResult(int fieldId)
        {
            var fieldToSave = new Field() { Id = fieldId };
            var message = $"Field with id = {fieldId} was removed.";

            fieldServiceMock
                .Setup(fieldService => fieldService.DeleteAsync(fieldId, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Success, Message = message });

            var result = await target.DeleteAsync(fieldId, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task DeleteAsync_WhenFieldIsProtected_ShouldReturn404NotFoundResult(int fieldId)
        {
            var message = $"Field with id = {fieldId} is protected and cannot be removed.";

            var fieldToSave = new Field();

            fieldServiceMock
                .Setup(fieldService => fieldService.DeleteAsync(fieldId, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Failed, Message = message });

            var result = await target.DeleteAsync(fieldId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-10)]
        public async Task DeleteAsync_WhenFieldIdIsLessOrEqualsToZero_ShouldReturnBadRequestObjectResult(int fieldId)
        {
            var result = await target.DeleteAsync(fieldId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(fieldId)} parameter has invalid value.");
        }

        [TestMethod]
        [DataRow("key1", 2)]
        [DataRow("key2", 3)]
        public void GetFieldTypes_ShouldReturnDictionary(string key, int value)
        {
            fieldServiceMock
                .Setup(fieldService => fieldService.GetFieldTypes())
                .Returns(new Dictionary<string, int>() { { key, value } });

            var result = target.GetFieldTypes();

            Assert.IsTrue(result.ContainsKey(key));
            Assert.IsTrue(result.ContainsValue(value));
        }

        [TestMethod]
        [DataRow("key1", 2)]
        [DataRow("key2", 3)]
        public void GetTargetFieldTypes_ShouldReturnDictionary(string key, int value)
        {
            fieldServiceMock
                .Setup(fieldService => fieldService.GetTargetFieldTypes())
                .Returns(new Dictionary<string, int>() { { key, value } });

            var result = target.GetTargetFieldTypes();

            Assert.IsTrue(result.ContainsKey(key));
            Assert.IsTrue(result.ContainsValue(value));
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType, string expectedMessage)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;

            Assert.IsNotNull(objectResult.Value);
            Assert.IsInstanceOfType(objectResult.Value, typeof(ApiResponse));

            var apiResponse = (ApiResponse)objectResult.Value;
            Assert.AreEqual(expectedMessage, apiResponse.Message);
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
        }

        private Mock<IFieldService> fieldServiceMock;
        private FieldsController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
