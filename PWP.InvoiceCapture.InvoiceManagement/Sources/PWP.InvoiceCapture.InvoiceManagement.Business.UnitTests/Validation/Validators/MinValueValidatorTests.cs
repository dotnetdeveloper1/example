using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MinValueValidatorTests
    { 
        [TestInitialize]
        public void Initialize()
        {
            target = new MinValueValidator();
        }

        [TestMethod]
        public void Validate_WhenEntityIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(null, 1, testFieldName));
        }

        [TestMethod]
        public void Validate_WhenEntityValueIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new Annotation() { FieldValue = null }, 1, testFieldName));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        public void Validate_WhenFieldNameIsEmpty_ShouldThrowArgumentNullException(string fieldName)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new Annotation() { FieldValue = "asdasd" }, 1, fieldName));
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(4)]
        public void Validate_WhenFieldValueMoreOrEqualsToMinValue_ShouldReturnOkResult(int value)
        {
            var result = target.Validate(new Annotation()
            {
                FieldValue = value.ToString(),
                FieldType = testFieldType
            }, 
            minValue,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(-1)]
        public void Validate_WhenFieldValueIsLessThanMinValue_ShouldReturnFailedResult(int value)
        {
            var result = target.Validate(new Annotation()
            {
                FieldValue = value.ToString(),
                FieldType = testFieldType
            }, 
            Convert.ToDecimal(minValue),
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        private MinValueValidator target;
        private readonly string validationResultMesage = $"Field {testFieldName} is less than {minValue}.";
        private const string testFieldType = "testField";
        private const string testFieldName = "testFieldName";
        private const decimal minValue = 3;
    }
}
