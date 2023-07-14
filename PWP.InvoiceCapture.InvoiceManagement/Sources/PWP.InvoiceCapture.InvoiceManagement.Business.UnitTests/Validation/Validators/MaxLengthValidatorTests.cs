using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MaxLengthValidatorTests
    { 
        [TestInitialize]
        public void Initialize()
        {
            target = new MaxLengthValidator();
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
        public void Validate_WhenFieldValueIsLessThanMaxLength_ShouldReturnOkResult()
        {
            var result = target.Validate(new Annotation()
            {
                FieldValue = testFieldValue,
                FieldType = testFieldType
            },
            maxLength,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsMoreThanMaxLength_ShouldReturnFailedResult()
        {
            var result = target.Validate(new Annotation()
            {
                FieldValue = longTestFieldValue,
                FieldType = testFieldType
            }, 
            maxLength,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        private MaxLengthValidator target;
        private readonly string validationResultMesage = $"Field {testFieldName} is more than {maxLength} characters.";
        private const string testFieldType = "testField";
        private const string testFieldName = "testFieldName";
        private const string testFieldValue= "te";
        private const string longTestFieldValue= "te st field";
        private const int maxLength = 3;
    }
}
