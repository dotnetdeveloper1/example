using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MinLengthValidatorTests
    { 
        [TestInitialize]
        public void Initialize()
        {
            target = new MinLengthValidator();
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
        [DataRow("test")]
        [DataRow("tes")]
        public void Validate_WhenFieldValueMoreOrEqualsToMinLength_ShouldReturnOkResult(string value)
        {
            var result = target.Validate(new Annotation()
            {
                FieldValue = value,
                FieldType = testFieldType
            },
            minLength,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsLessThanMinLength_ShouldReturnFailedResult()
        {
            var result = target.Validate(new Annotation()
            {
                FieldValue = shortTestFieldValue,
                FieldType = testFieldType
            }, 
            minLength,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        private MinLengthValidator target;
        private readonly string validationResultMesage = $"Field {testFieldName} is less than {minLength} characters.";
        private const string testFieldType = "testField";
        private const string testFieldName = "testFieldName";
        private const string shortTestFieldValue= "te";
        private const int minLength = 3;
    }
}
