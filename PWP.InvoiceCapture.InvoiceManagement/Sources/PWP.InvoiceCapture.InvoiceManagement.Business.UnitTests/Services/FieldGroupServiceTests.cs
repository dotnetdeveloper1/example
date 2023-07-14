using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FieldGroupServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            fieldGroupRepositoryMock = mockRepository.Create<IFieldGroupRepository>();
            fieldRepositoryMock = mockRepository.Create<IFieldRepository>();
            cancellationToken = CancellationToken.None;
            target = new FieldGroupService(fieldGroupRepositoryMock.Object, fieldRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenFieldGroupRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldGroupService(null, fieldRepositoryMock.Object));
        }
        [TestMethod]
        public void Instance_WhenFieldRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldGroupService(fieldGroupRepositoryMock.Object, null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteByFieldGroupIdAsync_WhenFieldGroupIdIsZeroOrLessThenZero_ShouldThrowArgumentException(int fieldGroupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(fieldGroupId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteAsync_WhenFieldGroupExists_ShouldDeleteFieldGroup(int fieldGroupId)
        {
            var expectedFieldGroup = new FieldGroup() { Id = fieldGroupId };

            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync(expectedFieldGroup);

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Field>() { new Field() });

            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.DeleteAsync(fieldGroupId, cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await target.DeleteAsync(fieldGroupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"FieldGroup with id = {fieldGroupId} was removed.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteAsync_WhenFieldGroupNotExists_DoNothing(int fieldGroupId)
        {
            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync((FieldGroup)null);

            var result = await target.DeleteAsync(fieldGroupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"FieldGroup with id = {fieldGroupId} already removed.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteAsync_WhenFieldGroupIsProtected_ShouidNotDeleteFieldGroup(int fieldGroupId)
        {
            var expectedfieldGroup = new FieldGroup() { Id = fieldGroupId, IsProtected = true };

            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync(expectedfieldGroup);
            
            var result = await target.DeleteAsync(fieldGroupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"FieldGroup with id = {fieldGroupId} is protected and cannot be removed.", result.Message);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteGroupFieldsAsync_WhenGroupIdIsZeroOrLessThenZero_ShouldThrowArgumentException(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteFieldsByGroupIdAsync(groupId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteGroupFieldsAsync_WhenFieldNotExists_ReturnSuccessOperationResult(int groupId)
        {
            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Field>() { new Field() { GroupId = groupId + 1 } });

            var result = await target.DeleteFieldsByGroupIdAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"No fields to remove for group with id = {groupId}.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteGroupFieldsAsync_WhenContainsProtectedField_ReturnFaildOperationResult(int groupId)
        {
            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Field>() { new Field() { GroupId = groupId, IsProtected = true } });

            var result = await target.DeleteFieldsByGroupIdAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Group with id = {groupId} contains at least one protected field.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteGroupFieldsAsync_WhenAllFieldsNotProtected_ReturnSuccessOperationResult(int groupId)
        {
            var fields = Enumerable.Range(1, 10)
                .Select(fieldId => new Field() { Id = fieldId, GroupId = groupId, IsProtected = false })
                .ToList();

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            var fieldIds = fields.Select(field => field.Id).ToList();

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.DeleteAsync(fieldIds, cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await target.DeleteFieldsByGroupIdAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"Fields were removed for group with id = {groupId}.", result.Message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetAsync_WhenFieldGroupIdIsLessOrEqualsZero_ShouldReturnFieldGroup(int fieldGroupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(fieldGroupId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task GetAsync_WhenFieldGroupExists_ShouldReturnFieldGroup(int fieldGroupId)
        {
            var expectedFieldGroup = new FieldGroup() { Id = fieldGroupId };

            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync(expectedFieldGroup);

            var actualFieldGroup = await target.GetAsync(fieldGroupId, cancellationToken);

            Assert.AreEqual(expectedFieldGroup, actualFieldGroup);
        }

        [TestMethod]
        public async Task GetListAsync_WhenFieldGroupsCollectionExists_ShouldReturnFieldGroups()
        {
            var expectedFieldGroups = new List<FieldGroup>() { new FieldGroup() };

            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedFieldGroups);

            var actualFields = await target.GetListAsync(cancellationToken);

            Assert.AreEqual(expectedFieldGroups, actualFields);
        }

        [TestMethod]
        public void CreateAsync_WhenFieldGroupIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenFieldGroupIsNotNull_ShouldCreateFieldGroup()
        {
            var expectedfieldGroup = new FieldGroup();

            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.CreateAsync(expectedfieldGroup, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.CreateAsync(expectedfieldGroup, cancellationToken);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenFieldGroupIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateAsync_WhenFieldGroupIdIsLessOrEqualsZero_ShouldThrowArgumentException(int fieldGroupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateAsync(fieldGroupId, new FieldGroup(), cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateAsync_WhenFieldGroupIsNotNull_ShouldUpdateFieldGroup(int fieldGroupId)
        {
            var fieldGroup = new FieldGroup() { Id = fieldGroupId };

            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync(fieldGroup);

            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.UpdateAsync(fieldGroupId, fieldGroup, cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await target.UpdateAsync(fieldGroup.Id, fieldGroup, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"FieldGroup with id = {fieldGroupId} was updated.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateAsync_WhenFieldGroupNotExists_ShouldDoNothing(int fieldGroupId)
        {
            var fieldGroup = new FieldGroup() { Id = fieldGroupId };

            fieldGroupRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldGroupId, cancellationToken))
                .ReturnsAsync((FieldGroup)null);

            var result = await target.UpdateAsync(fieldGroupId, fieldGroup, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.NotFound, result.Status);
            Assert.AreEqual($"FieldGroup with id = {fieldGroupId} not found.", result.Message);
        }

        private MockRepository mockRepository;
        private Mock<IFieldGroupRepository> fieldGroupRepositoryMock;
        private Mock<IFieldRepository> fieldRepositoryMock;
        private FieldGroupService target;
        private CancellationToken cancellationToken;
    }
}
