using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DecimalValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new DecimalValidator();
        }

        [TestMethod]
        public void Validate_WhenEntityIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(null, cultureInfo, testFieldName));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public void Validate_WhenFieldNameIsEmpty_ShouldThrowArgumentNullException(string fieldName)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new Annotation() { FieldValue = "dsf" }, cultureInfo, fieldName));
        }

        [TestMethod]
        public void Validate_WhenEntityValueIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new Annotation() { FieldValue = null }, cultureInfo, testFieldName));
        }

        [TestMethod]
        public void Validate_WhenEntityFieldValueIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new Annotation() { FieldValue = null }, cultureInfo, testFieldName));
        }

        [TestMethod]
        public void Validate_WhenFieldValueNotDecimal_ShouldReturnFailedResult()
        {
            var result =  target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = invalidDecimal,
            }, 
            cultureInfo,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsCorrectDecimal_ShouldReturnValidationResultOk()
        {
            var result =  target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = validUsDecimal,
            }, 
            cultureInfo,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [DataRow(validFrenchDecimal)]
        [TestMethod]
        public void Validate_WhenFieldValueIsCorrectFrenchDecimal_ShouldReturnValidationResultOk(string value)
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = value,
            }, 
            frenchCultureInfo,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsNotCorrectFrenchDecimal_ShouldReturnFailedResult()
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = validUsDecimal,
            }, 
            frenchCultureInfo, 
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        private DecimalValidator target;
        private readonly string validationResultMesage = $"Field {testFieldName} can't be converted to decimal.";
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        private readonly CultureInfo frenchCultureInfo = new CultureInfo("fr-FR");

        private const string testFieldType = "testField";
        private const string testFieldName = "testFieldName";
        private const string validUsDecimal = "16,325.62";
        private const string invalidDecimal = "189.r15";

        private const string validFrenchDecimal = "189,15";
    }
}
