using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MaxValueValidatorTests
    { 
        [TestInitialize]
        public void Initialize()
        {
            target = new MaxValueValidator();
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
        [DataRow(2)]
        public void Validate_WhenFieldValueLessOrEqualsToMaxValue_ShouldReturnOkResult(int value)
        {
            var result = target.Validate(new Annotation()
            {
                FieldValue = value.ToString(),
                FieldType = testFieldType
            },
            Convert.ToDecimal(maxValue),
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        [DataRow(5)]
        [DataRow(4)]
        public void Validate_WhenFieldValueIsMoreThanMaxValue_ShouldReturnFailedResult(int value)
        {
            var result = target.Validate(new Annotation()
            {
                FieldValue = value.ToString(),
                FieldType = testFieldType
            }, 
            Convert.ToDecimal(maxValue),
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        private MaxValueValidator target;
        private readonly string validationResultMesage = $"Field {testFieldName} is more than {maxValue}.";
        private const string testFieldType = "testField";
        private const string testFieldName = "testFieldName";
        private const decimal maxValue = 3;
    }
}
