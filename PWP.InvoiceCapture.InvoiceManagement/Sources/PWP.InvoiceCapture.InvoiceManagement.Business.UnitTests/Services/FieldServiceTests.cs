using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
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
    public class FieldServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            fieldRepositoryMock = mockRepository.Create<IFieldRepository>();
            fieldGroupServiceMock = mockRepository.Create<IFieldGroupService>();
            formulaValidatorMock = mockRepository.Create<IFormulaValidator>();
            formulaFieldRepositoryMock = mockRepository.Create<IFormulaFieldRepository>();
            formulaExtractionServiceMock = mockRepository.Create<IFormulaExtractionService>();
            cancellationToken = CancellationToken.None;
            target = new FieldService(fieldRepositoryMock.Object,
                fieldGroupServiceMock.Object,
                formulaValidatorMock.Object,
                formulaFieldRepositoryMock.Object,
                formulaExtractionServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenFieldRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldService(null,
                fieldGroupServiceMock.Object,
                formulaValidatorMock.Object,
                formulaFieldRepositoryMock.Object,
                formulaExtractionServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenFieldGroupServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldService(fieldRepositoryMock.Object,
                null,
                formulaValidatorMock.Object,
                formulaFieldRepositoryMock.Object,
                formulaExtractionServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenFormulaValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldService(fieldRepositoryMock.Object,
                fieldGroupServiceMock.Object,
                null,
                formulaFieldRepositoryMock.Object,
                formulaExtractionServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenFormulaFieldRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldService(fieldRepositoryMock.Object,
                fieldGroupServiceMock.Object,
                formulaValidatorMock.Object,
                null,
                formulaExtractionServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenFormulaExtractionServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldService(fieldRepositoryMock.Object,
                fieldGroupServiceMock.Object,
                formulaValidatorMock.Object,
                formulaFieldRepositoryMock.Object,
                null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteByFieldIdAsync_WhenFieldIdIsZeroOrLessThenZero_ShouldThrowArgumentException(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(fieldId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteAsync_WhenFieldExists_ShouldReturnField(int fieldId)
        {
            var expectedField = new Field() { Id = fieldId };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldId, cancellationToken))
                .ReturnsAsync(expectedField);

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.DeleteAsync(fieldId, cancellationToken))
                .Returns(Task.CompletedTask);

            formulaFieldRepositoryMock
                .Setup(formulaFieldRepository => formulaFieldRepository.UsedAsOperandInFormulaAsync(fieldId, cancellationToken))
                .ReturnsAsync(false);

            formulaFieldRepositoryMock
                .Setup(formulaFieldRepository => formulaFieldRepository.DeleteAllByResultFieldIdAsync(fieldId, cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await target.DeleteAsync(fieldId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"Field with id = {fieldId} was removed.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteAsync_WhenFieldNotExists_DoNothing(int fieldId)
        {
            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldId, cancellationToken))
                .ReturnsAsync((Field)null);

            var result = await target.DeleteAsync(fieldId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"Field with id = {fieldId} already removed.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteAsync_WhenFieldIsProtected_ShouldReturnField(int fieldId)
        {
            var expectedField = new Field() { Id = fieldId, IsProtected = true };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldId, cancellationToken))
                .ReturnsAsync(expectedField);

            var result = await target.DeleteAsync(fieldId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Field with id = {fieldId} is protected and cannot be removed.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteAsync_WhenFieldUsedInOtherFormula_ShouldReturnFailedResult(int fieldId)
        {
            var expectedField = new Field() { Id = fieldId, IsProtected = false };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldId, cancellationToken))
                .ReturnsAsync(expectedField);

            formulaFieldRepositoryMock
                .Setup(formulaFieldRepository => formulaFieldRepository.UsedAsOperandInFormulaAsync(fieldId, cancellationToken))
                .ReturnsAsync(true);

            var result = await target.DeleteAsync(fieldId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Field with id = {fieldId} is used in custom validation rule and cannot be removed.", result.Message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetAsync_WhenFieldIdIsLessOrEqualsZero_ShouldReturnField(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(fieldId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task GetAsync_WhenFieldExists_ShouldReturnField(int fieldId)
        {
            var expectedField = new Field() { Id = fieldId };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(fieldId, cancellationToken))
                .ReturnsAsync(expectedField);

            var actualField = await target.GetAsync(fieldId, cancellationToken);

            Assert.AreEqual(expectedField, actualField);
        }

        [TestMethod]
        public async Task GetListAsync_WhenFieldsCollectionExists_ShouldReturnFields()
        {
            var expectedFields = new List<Field>() { new Field() };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedFields);

            var actualFields = await target.GetListAsync(cancellationToken);

            Assert.AreEqual(expectedFields, actualFields);
        }

        [TestMethod]
        public void CreateAsync_WhenFieldIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow("DisplayName1")]
        [DataRow("DisplayName2")]
        public async Task CreateAsync_WhenFieldIsNotNull_ShouldCreateField(string displayName)
        {
            var expectedField = new Field() { GroupId = 1, DisplayName = displayName };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.CreateAsync(expectedField, cancellationToken))
                .Returns(Task.CompletedTask);

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(expectedField.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            var result = await target.CreateAsync(expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"Field with name = {displayName} was created.", result.Message);
        }

        [TestMethod]
        [DataRow("[1]+[2]")]
        [DataRow("[2]-[1]")]
        public async Task CreateAsync_WhenHasCorrectFormula_ShouldCreateField(string formula)
        {
            var expectedField = new Field() { GroupId = 1, DisplayName = "Field Name", Formula = formula };
            var fields = new List<Field>() { new Field { Id = 1 }, new Field { Id = 2 } };
            var extractedFieldsIds = new List<int> { 1, 2 };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.CreateAsync(expectedField, cancellationToken))
                .Returns(Task.CompletedTask);

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(expectedField.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(formula))
                .Returns(extractedFieldsIds);

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            formulaValidatorMock
                .Setup(formulaValidator => formulaValidator.Validate(fields, expectedField, false))
                .Returns(ValidationResult.Ok);

            formulaFieldRepositoryMock
                .Setup(formulaFieldRepository => formulaFieldRepository.CreateAsync(expectedField.Id, extractedFieldsIds, cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await target.CreateAsync(expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"Field with name = {expectedField.DisplayName} was created.", result.Message);
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow("(2]-[1}]")]
        public async Task CreateAsync_WhenHasIncorrectFormula_ShouldReturnFailedResult(string formula)
        {
            var expectedField = new Field() { GroupId = 1, DisplayName = "Field Name", Formula = formula };
            var fields = new List<Field>() { new Field { Id = 1 }, new Field { Id = 2 } };
            var errorMessage = "Invalid validation formula.Can't parse field id.";

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(expectedField.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(formula))
                .Returns(new List<int> { 1, 2 });

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            formulaValidatorMock
                .Setup(formulaValidator => formulaValidator.Validate(fields, expectedField, false))
                .Returns(ValidationResult.Failed(errorMessage));

            var result = await target.CreateAsync(expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual(errorMessage, result.Message);
        }

        [TestMethod]
        [DataRow(FieldType.String)]
        [DataRow(FieldType.DateTime)]
        public async Task CreateAsync_WhenHasCorrectFormulaButNotDecimalType_ShouldReturnFaildResult(FieldType type)
        {
            var expectedField = new Field() { Formula = "[1]+[2]", Type = type };
            var fields = new List<Field>() { new Field { Id = 1 }, new Field { Id = 2 } };
            var errorMessage = "Can't set validation formula for non-decimal field.";

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(expectedField.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(expectedField.Formula))
                .Returns(new List<int> { 1, 2 });

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            formulaValidatorMock
                .Setup(formulaValidator => formulaValidator.Validate(fields, expectedField, false))
                .Returns(ValidationResult.Failed(errorMessage));

            var result = await target.CreateAsync(expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual(errorMessage, result.Message);
        }

        [TestMethod]
        [DataRow("DisplayName1")]
        [DataRow("DisplayName2")]
        public async Task CreateAsync_WhenGroupNotExists_ShouldReturnFaildResult(string displayName)
        {
            var expectedField = new Field() { GroupId = 1, DisplayName = displayName };

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(expectedField.GroupId, cancellationToken))
                .ReturnsAsync((FieldGroup)null);

            var result = await target.CreateAsync(expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Group with id = {expectedField.GroupId} not exists.", result.Message);
        }

        [TestMethod]
        [DataRow("DisplayName1", TargetFieldType.AmountDue)]
        [DataRow("DisplayName2", TargetFieldType.CustomerAddressRecipien)]
        public async Task CreateAsync_WhenFieldHasUniqueTargetFieldType_ShouldCreateField(string displayName, TargetFieldType targetFieldType)
        {
            var expectedField = new Field() { DisplayName = displayName, TargetFieldType = targetFieldType };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.CreateAsync(expectedField, cancellationToken))
                .Returns(Task.CompletedTask);

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Field>() { new Field { Id = 100, TargetFieldType = TargetFieldType.CustomerId } });

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(expectedField.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            var result = await target.CreateAsync(expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"Field with name = {displayName} was created.", result.Message);
        }

        [TestMethod]
        [DataRow("DisplayName1", TargetFieldType.AmountDue)]
        [DataRow("DisplayName2", TargetFieldType.CustomerAddressRecipien)]
        public async Task CreateAsync_WhenTargetFieldAlreadyAssigned_ShouldReturnFailOperationResult(string displayName, TargetFieldType targetFieldType)
        {
            var expectedField = new Field() { DisplayName = displayName, TargetFieldType = targetFieldType };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Field>() { new Field { Id = 100, TargetFieldType = targetFieldType } });

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(expectedField.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            var result = await target.CreateAsync(expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Target Field with value = {(int)targetFieldType} already assigned to another field.", result.Message);
        }

        [TestMethod]
        [DataRow("DisplayName1", 1000)]
        [DataRow("DisplayName2", -1000)]
        public async Task CreateAsync_WhenTargetFieldTypeNotDefined_ShouldReturnFailedResult(string displayName, int targetFieldTypeValue)
        {
            var expectedField = new Field() { DisplayName = displayName, TargetFieldType = (TargetFieldType)targetFieldTypeValue };

            var result = await target.CreateAsync(expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Field contains unknown target field type.", result.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenFieldIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateAsync_WhenFieldIdIsLessOrEqualsZero_ShouldThrowArgumentException(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateAsync(fieldId, new Field(), cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateAsync_WhenFieldExists_ShouldUpdateField(int fieldId)
        {
            var field = new Field() { Id = fieldId, GroupId = 1 };
            var fields = new List<Field>() { new Field() { Id = fieldId + 1 } };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(field.Id, cancellationToken))
                .ReturnsAsync(field);

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.UpdateAsync(field.Id, field, cancellationToken))
                .Returns(Task.CompletedTask);

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(field.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            formulaFieldRepositoryMock
                .Setup(formulaFieldRepository => formulaFieldRepository.GetByOperandFieldIdAsync(fieldId, cancellationToken))
                .ReturnsAsync(new List<FormulaField>());

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            formulaValidatorMock
                .Setup(formulaValidator => formulaValidator.Validate(fields, field, false))
                .Returns(ValidationResult.Ok);

            var result = await target.UpdateAsync(field.Id, field, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"Field with id = {fieldId} was updated.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateAsync_WhenFieldGroupNotExists_FieldShouldNotBeUpdated(int fieldId)
        {
            var field = new Field() { Id = fieldId, GroupId = 1 };

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(field.GroupId, cancellationToken))
                .ReturnsAsync((FieldGroup)null);

            var result = await target.UpdateAsync(field.Id, field, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Group with id = {field.GroupId} not exists.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateAsync_WhenFieldUsedInFormulaAndFieldTypeChandedFromDecimal_FieldShouldNotBeUpdated(int fieldId)
        {
            var field = new Field() { Id = fieldId, Type = FieldType.DateTime };
            var fields = new List<Field>() { new Field() { Id = fieldId + 1 } };
            var errorMessage = "Can't change field type. Field is used as operand in other field validation formula.";

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(field.Id, cancellationToken))
                .ReturnsAsync(field);

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(field.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            formulaFieldRepositoryMock
                .Setup(formulaFieldRepository => formulaFieldRepository.GetByOperandFieldIdAsync(fieldId, cancellationToken))
                .ReturnsAsync(new List<FormulaField> { new FormulaField { OperandFieldId = fieldId } });

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            formulaValidatorMock
                .Setup(formulaValidator => formulaValidator.Validate(fields, field, true))
                .Returns(ValidationResult.Failed(errorMessage));



            var result = await target.UpdateAsync(field.Id, field, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual(errorMessage, result.Message);
        }

        [TestMethod]
        [DataRow(1, TargetFieldType.CustomerId)]
        [DataRow(2, TargetFieldType.ServiceAddress)]
        public async Task UpdateAsync_WhenFieldHasUniqueFieldType_ShouldUpdateField(int fieldId, TargetFieldType targetFieldType)
        {
            var field = new Field() { Id = fieldId, GroupId = 1, TargetFieldType = targetFieldType };
            var fields = new List<Field>() { new Field() { Id = fieldId + 1 } };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(field.Id, cancellationToken))
                .ReturnsAsync(field);

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.UpdateAsync(field.Id, field, cancellationToken))
                .Returns(Task.CompletedTask);

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Field>() { new Field() { Id = fieldId + 1, TargetFieldType = TargetFieldType.ServiceStartDate } });

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(field.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            formulaFieldRepositoryMock
                .Setup(formulaFieldRepository => formulaFieldRepository.GetByOperandFieldIdAsync(fieldId, cancellationToken))
                .ReturnsAsync(new List<FormulaField>());

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(fields);

            formulaValidatorMock
                .Setup(formulaValidator => formulaValidator.Validate(fields, field, false))
                .Returns(ValidationResult.Ok);

            var result = await target.UpdateAsync(field.Id, field, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual($"Field with id = {fieldId} was updated.", result.Message);
        }

        [TestMethod]
        [DataRow(1, TargetFieldType.CustomerId)]
        [DataRow(2, TargetFieldType.ServiceAddress)]
        public async Task UpdateAsync_WhenTargetFieldAlreadyAssigned_ShouldReturnFailOperationResult(int fieldId, TargetFieldType targetFieldType)
        {
            var field = new Field() { Id = fieldId, GroupId = 1, TargetFieldType = targetFieldType };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Field>() { new Field() { Id = fieldId + 1, TargetFieldType = targetFieldType } });

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(field.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            var result = await target.UpdateAsync(field.Id, field, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Target Field with value = {(int)targetFieldType} already assigned to another field.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateAsync_WhenFieldNotExists_ShouldDoNothing(int fieldId)
        {
            var field = new Field() { Id = fieldId, GroupId = 1 };

            fieldRepositoryMock
                .Setup(fieldRepository => fieldRepository.GetAsync(field.Id, cancellationToken))
                .ReturnsAsync((Field)null);

            fieldGroupServiceMock
                .Setup(fieldGroupService => fieldGroupService.GetAsync(field.GroupId, cancellationToken))
                .ReturnsAsync(new FieldGroup());

            var result = await target.UpdateAsync(field.Id, field, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.NotFound, result.Status);
            Assert.AreEqual($"Field with id = {fieldId} not found.", result.Message);
        }

        [TestMethod]
        [DataRow(1, 1000)]
        [DataRow(2, -1000)]
        public async Task UpdateAsync_WhenTargetFieldTypeNotDefined_ShouldReturnFailedResult(int fieldId, int targetFieldTypeValue)
        {
            var expectedField = new Field() { Id = fieldId, TargetFieldType = (TargetFieldType)targetFieldTypeValue };

            var result = await target.UpdateAsync(fieldId, expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Field contains unknown target field type.", result.Message);
        }

        [TestMethod]
        public void GetFieldTypes_ShouldReturnDictionary()
        {
            var result = target.GetFieldTypes();

            AssertEnumDictionary<FieldType>(result);
        }

        [TestMethod]
        public void GetTargetFieldTypes_ShouldReturnDictionary()
        {
            var result = target.GetTargetFieldTypes();

            AssertEnumDictionary<TargetFieldType>(result);
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
        private Mock<IFieldRepository> fieldRepositoryMock;
        private Mock<IFieldGroupService> fieldGroupServiceMock;
        private Mock<IFormulaValidator> formulaValidatorMock;
        private Mock<IFormulaFieldRepository> formulaFieldRepositoryMock;
        private Mock<IFormulaExtractionService> formulaExtractionServiceMock;
        private FieldService target;
        private CancellationToken cancellationToken;
    }
}
