using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.API.Versions.Neutral.Controllers;
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
    public class FieldGroupsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            fieldGroupServiceMock = mockRepository.Create<IFieldGroupService>();
            target = new FieldGroupsController(fieldGroupServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenFieldGroupServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldGroupsController(null));
        }

        [TestMethod]
        public async Task GetFieldGroupListAsync_WhenFieldGroupsCollectionExists_ShouldReturnOkResultWithFieldGroupList()
        {
            var expectedFieldGroups = new List<FieldGroup> { new FieldGroup() };

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedFieldGroups);

            var result = await target.GetFieldGroupsListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<FieldGroup>>;

            Assert.IsInstanceOfType(apiResponce.Data, expectedFieldGroups.GetType());
            Assert.AreEqual(expectedFieldGroups, apiResponce.Data);
        }

        [TestMethod]
        public async Task GetFieldGroupListAsync_WhenFieldGroupsNotFound_ShouldReturnOkResultWithNull()
        {
            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetListAsync(cancellationToken))
                .ReturnsAsync((List<FieldGroup>)null);

            var result = await target.GetFieldGroupsListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<FieldGroup>>;

            Assert.IsNull(apiResponce.Data);
        }

        [TestMethod]
        public async Task GetFieldGroupListAsync_WhenFieldGroupsWereFound_ShouldReturnOkResult()
        {
            var expectedFieldGroups = new List<FieldGroup>() { new FieldGroup() };

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedFieldGroups);

            var result = await target.GetFieldGroupsListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<FieldGroup>>;

            Assert.IsNotNull(apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetFieldGroupByIdAsync_WhenFieldGroupFound_ShouldReturnOkResultWithFieldGroup(int fieldGroupId)
        {
            var expectedFieldGroup = new FieldGroup();

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync(expectedFieldGroup);

            var result = await target.GetFieldGroupByIdAsync(fieldGroupId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<FieldGroup>;

            Assert.IsNotNull(apiResponce);
            Assert.IsInstanceOfType(apiResponce.Data, expectedFieldGroup.GetType());
            Assert.AreEqual(expectedFieldGroup, apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetFieldGroupByIdAsync_WhenFieldGroupNotFound_ShouldReturn404NotFoundResult(int fieldGroupId)
        {
            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync((FieldGroup)null);

            var result = await target.GetFieldGroupByIdAsync(fieldGroupId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"FieldGroup with id = {fieldGroupId} not found.");
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetFieldGroupByIdAsync_WhenFieldGroupIdParameterOutOfRange_ShouldReturnBadRequestResult(int fieldGroupId)
        {
            var result = await target.GetFieldGroupByIdAsync(fieldGroupId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(fieldGroupId)} parameter has invalid value.");
        }

        [TestMethod]
        public async Task CreateAsync_WhenFieldGroupExists_ShouldReturnOkObjectResult()
        {
            var fieldGroupToSave = new FieldGroup();

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.CreateAsync(fieldGroupToSave, cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await target.CreateFieldGroupAsync(fieldGroupToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(OkObjectResult));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateFieldGroupAsync_WhenFieldGroupExists_ShouldReturnOkObjectResult(int fieldGroupId)
        {
            var fieldGroupToSave = new FieldGroup() { Id = fieldGroupId };
            var message = $"FieldGroup with id = {fieldGroupId} was updated.";

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.UpdateAsync(fieldGroupId, fieldGroupToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Success, Message = message });

            var result = await target.UpdateFieldGroupAsync(fieldGroupId, fieldGroupToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateFieldGroupAsync_WhenFieldGroupNotFound_ShouldReturnBadRequestObjectResult(int fieldGroupId)
        {
            var fieldGroupToSave = new FieldGroup() { Id = fieldGroupId };
            var message = $"FieldGroup with id = {fieldGroupId} not found.";

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.UpdateAsync(fieldGroupId, fieldGroupToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.NotFound, Message = message });

            var result = await target.UpdateFieldGroupAsync(fieldGroupId, fieldGroupToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-10)]
        public async Task UpdateFieldGroupAsync_WhenFieldGroupIdIsLessOrEqualstoZeroNotFound_ShouldReturnBadRequestObjectResult(int fieldGroupId)
        {
            var fieldGroupToSave = new FieldGroup() { Id = fieldGroupId };

            var result = await target.UpdateFieldGroupAsync(fieldGroupId, fieldGroupToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(fieldGroupId)} parameter has invalid value.");
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task DeleteAsync_WhenFieldGroupWasRemoved_ShouldReturnOkObjectResult(int fieldGroupId)
        {
            var fieldGroupToSave = new FieldGroup() { Id = fieldGroupId };

            var message = $"FieldGroup with id = {fieldGroupId} was removed.";

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.DeleteAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Success, Message = message });

            var result = await target.DeleteFieldGroupAsync(fieldGroupId, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task DeleteAsync_WhenFieldGroupIsProtected_ShouldReturnBadRequestResult(int fieldGroupId)
        {
            var message = $"FieldGroup with id = {fieldGroupId} is protected and cannot be removed.";

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.DeleteAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Failed, Message = message });

            var result = await target.DeleteFieldGroupAsync(fieldGroupId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-10)]
        public async Task DeleteAsync_WhenFieldGroupIdIsLessOrEqualstoZeroNotFound_ShouldReturnBadRequestObjectResult(int fieldGroupId)
        {
            var result = await target.DeleteFieldGroupAsync(fieldGroupId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(fieldGroupId)} parameter has invalid value.");
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

        private Mock<IFieldGroupService> fieldGroupServiceMock;
        private FieldGroupsController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
