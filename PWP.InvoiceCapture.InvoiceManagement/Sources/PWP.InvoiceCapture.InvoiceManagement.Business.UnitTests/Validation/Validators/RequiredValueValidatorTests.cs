using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RequiredValueValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new RequiredValueValidator();
        }

        [TestMethod]
        public void Validate_WhenEntityIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(null, testFieldName));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        public void Validate_WhenFieldNameIsEmpty_ShouldThrowArgumentNullException(string fieldName)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new Annotation() { FieldValue = "asdasd" }, fieldName));
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsEmptyString_ShouldReturnFailedResult()
        {
            var result = target.Validate(new Annotation() 
            { 
                FieldType = testFieldType,
                FieldValue = string.Empty
            },
            testFieldName);
            
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        [TestMethod]
        public void Validate_WhenFieldValueNotEmpty_ShouldReturnValidationResultOk()
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = testFieldValue
            },
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        private RequiredValueValidator target;
        private readonly string validationResultMesage = $"Field {testFieldName} is empty.";
        private const string testFieldType = "testField";
        private const string testFieldName = "testFieldName";
        private const string testFieldValue = "some value";
    }
}
