using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FormulaValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            formulaExtractionServiceMock = mockRepository.Create<IFormulaExtractionService>();
            target = new FormulaValidator(formulaExtractionServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenFormulaExtractionServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FormulaValidator(null));
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(4)]
        public void Validate_WhenFieldIsDecimalAndFormulaIsValidAndUsedAsOperand_ShouldPassValidation(int fieldId)
        {
            var field = new Field { Id = fieldId, Type = FieldType.Decimal, Formula = validFormula };
            var fieldsIds = new List<int> { 1, 2 };
            var fieldValues = new Dictionary<int, decimal> { { 1, 1 }, { 2, 1 } };

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.AreSquareBracketsBalanced(field.Formula))
                .Returns(true);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(field.Formula))
                .Returns(fieldsIds);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetNormalizedFormula(field.Formula, fieldValues))
                .Returns(validFieldsIdsString);

            var result = target.Validate(fields, field, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(4)]
        public void Validate_WhenFieldIsDecimalAndFormulaIsNotValidAndUsedAsOperand_ShouldPassValidation(int fieldId)
        {
            var field = new Field { Id = fieldId, Type = FieldType.Decimal, Formula = invalidFormula };
            var fieldsIds = new List<int> { 1, 2 };
            var fieldValues = new Dictionary<int, decimal> { { 1, 1 }, { 2, 1 } };

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.AreSquareBracketsBalanced(field.Formula))
                .Returns(true);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(field.Formula))
                .Returns(fieldsIds);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetNormalizedFormula(field.Formula, fieldValues))
                .Returns(invalidFieldsIdsString);

            var result = target.Validate(fields, field, true);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Invalid validation formula. Can't calculate formula expression.", result.Message);
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(4)]
        public void Validate_WhenFieldIsDecimalAndFormulaIsNullAndUsedAsOperand_ShouldPassValidation(int fieldId)
        {
            var field = new Field { Id = fieldId, Type = FieldType.Decimal, Formula = null };

            var result = target.Validate(fields, field, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(4)]
        public void Validate_WhenFieldIsDecimalAndFormulaIsValidAndNotUsedAsOperand_ShouldPassValidation(int fieldId)
        {
            var field = new Field { Id = fieldId, Type = FieldType.Decimal, Formula = validFormula };
            var fieldsIds = new List<int> { 1, 2 };
            var fieldValues = new Dictionary<int, decimal> { { 1, 1 }, { 2, 1 } };

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.AreSquareBracketsBalanced(field.Formula))
                .Returns(true);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(field.Formula))
                .Returns(fieldsIds);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetNormalizedFormula(field.Formula, fieldValues))
                .Returns(validFieldsIdsString);

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(4)]
        public void Validate_WhenFieldIsDecimalAndFormulaIsNotValidAndNotUsedAsOperand_ShouldPassValidation(int fieldId)
        {
            var field = new Field { Id = fieldId, Type = FieldType.Decimal, Formula = invalidFormula };
            var fieldsIds = new List<int> { 1, 2 };
            var fieldValues = new Dictionary<int, decimal> { { 1, 1 }, { 2, 1 } };

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.AreSquareBracketsBalanced(field.Formula))
                .Returns(true);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(field.Formula))
                .Returns(fieldsIds);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetNormalizedFormula(field.Formula, fieldValues))
                .Returns(invalidFieldsIdsString);

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Invalid validation formula. Can't calculate formula expression.", result.Message);
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(4)]
        public void Validate_WhenFieldIsDecimalAndFormulaIsNullAndNotUsedAsOperand_ShouldPassValidation(int fieldId)
        {
            var field = new Field { Id = fieldId, Type = FieldType.Decimal, Formula = null };

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        [DataRow(FieldType.String)]
        [DataRow(FieldType.DateTime)]
        public void Validate_WhenFieldIsNotDecimalAndFormulaIsValidAndUsedAsOperand_ShouldNotPassValidation(FieldType fieldType)
        {
            var field = new Field { Id = 3, Type = fieldType, Formula = validFormula };

            var result = target.Validate(fields, field, true);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Can't change field type. Field is used as operand in other field validation formula.", result.Message);
        }

        [TestMethod]
        [DataRow(FieldType.String)]
        [DataRow(FieldType.DateTime)]
        public void Validate_WhenFieldIsNotDecimalAndFormulaIsNotValidAndUsedAsOperand_ShouldNotPassValidation(FieldType fieldType)
        {
            var field = new Field { Id = 3, Type = fieldType, Formula = validFormula };

            var result = target.Validate(fields, field, true);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Can't change field type. Field is used as operand in other field validation formula.", result.Message);
        }

        [TestMethod]
        [DataRow(FieldType.String)]
        [DataRow(FieldType.DateTime)]
        public void Validate_WhenFieldIsNotDecimalAndFormulaIsNullAndUsedAsOperand_ShouldNotPassValidation(FieldType fieldType)
        {
            var field = new Field { Id = 3, Type = fieldType, Formula = null };

            var result = target.Validate(fields, field, true);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Can't change field type. Field is used as operand in other field validation formula.", result.Message);
        }

        [TestMethod]
        [DataRow(FieldType.String)]
        [DataRow(FieldType.DateTime)]
        public void Validate_WhenFieldIsNotDecimalAndFormulaIsValidAndNotUsedAsOperand_ShouldNotPassValidation(FieldType fieldType)
        {
            var field = new Field { Id = 3, Type = fieldType, Formula = validFormula };

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Can't set validation formula for non-decimal field.", result.Message);
        }

        [TestMethod]
        [DataRow(FieldType.String)]
        [DataRow(FieldType.DateTime)]
        public void Validate_WhenFieldIsNotDecimalAndFormulaIsNotValidAndNotUsedAsOperand_ShouldNotPassValidation(FieldType fieldType)
        {
            var field = new Field { Id = 3, Type = fieldType, Formula = invalidFormula };

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Can't set validation formula for non-decimal field.", result.Message);
        }

        [TestMethod]
        [DataRow(FieldType.String)]
        [DataRow(FieldType.DateTime)]
        public void Validate_WhenFieldIsNotDecimalAndFormulaIsNullAndNotUsedAsOperand_ShouldPassValidation(FieldType fieldType)
        {
            var field = new Field { Id = 3, Type = fieldType, Formula = null };

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        [DataRow("[[1]+[2]+3")]
        [DataRow("3+[1]*[2]]")]
        public void Validate_WhenSquareBracketsNotBalanced_ShouldNotPassValidation(string formula)
        {
            var field = new Field { Id = 3, Type = FieldType.Decimal, Formula = formula };

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.AreSquareBracketsBalanced(field.Formula))
                .Returns(false);

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Invalid validation formula. Square brackets is not balanced.", result.Message);
        }

        [TestMethod]
        [DataRow("[1]/[2]+3")]
        [DataRow("3+[1]/[2]-7")]
        public void Validate_WhenFormulaContainsDivisionSign_ShouldNotPassValidation(string formula)
        {
            var field = new Field { Id = 3, Type = FieldType.Decimal, Formula = formula };
            var fieldsIds = new List<int> { 1, 2 };

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.AreSquareBracketsBalanced(field.Formula))
                .Returns(true);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(field.Formula))
                .Returns(fieldsIds);

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Invalid validation formula. Division operation is not allowed.", result.Message);
        }

        [TestMethod]
        [DataRow(5)]
        [DataRow(6)]
        public void Validate_WhenFormulaFieldsDontExist_ShouldNotPassValidation(int fieldId)
        {
            var field = new Field { Id = fieldId, Type = FieldType.Decimal, Formula = "[1]-[2]*[3]" };
            var fieldsIds = new List<int> { 1, 2, 3 };
            var fieldValues = new Dictionary<int, decimal> { { 1, 1 }, { 2, 1 } };

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.AreSquareBracketsBalanced(field.Formula))
                .Returns(true);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(field.Formula))
                .Returns(fieldsIds);

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Invalid validation formula. Field with specified id(s) doesn't exist.", result.Message);
        }

        [TestMethod]
        [DataRow(5)]
        [DataRow(6)]
        public void Validate_WhenFormulaHasNotDecimalType_ShouldNotPassValidation(int fieldId)
        {
            var field = new Field { Id = fieldId, Type = FieldType.Decimal, Formula = "[1]-[2]*3" };
            var fields = new List<Field> { new Field { Id = 1, Type = FieldType.Decimal }, new Field { Id = 2, Type = FieldType.String } };
            var fieldsIds = new List<int> { 1, 2 };
            var fieldValues = new Dictionary<int, decimal> { { 1, 1 }, { 2, 1 } };

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.AreSquareBracketsBalanced(field.Formula))
                .Returns(true);

            formulaExtractionServiceMock
                .Setup(formulaExtractionService => formulaExtractionService.GetFieldIds(field.Formula))
                .Returns(fieldsIds);

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Invalid validation formula. The type of one or more formula fields is different from decimal.", result.Message);
        }

        [TestMethod]
        [DataRow(FieldType.String)]
        [DataRow(FieldType.DateTime)]
        public void Validate_WhenFormulaIsNullOrEmptyAndFieldNotUsedAsOperand_ShouldPassValidation(FieldType fieldType)
        {
            var field = new Field { Id = 3, Type = fieldType };

            var result = target.Validate(fields, field, false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        private FormulaValidator target;
        private MockRepository mockRepository;
        private Mock<IFormulaExtractionService> formulaExtractionServiceMock;

        private readonly List<Field> fields = new List<Field> { new Field { Id = 1, Type = FieldType.Decimal }, new Field { Id = 2, Type = FieldType.Decimal } };
        private readonly string validFormula = "[1]+[2]";
        private readonly string validFieldsIdsString = "1+2";
        private readonly string invalidFormula = "{[)1]+[2]";
        private readonly string invalidFieldsIdsString = "{)1+2";
    }
}
