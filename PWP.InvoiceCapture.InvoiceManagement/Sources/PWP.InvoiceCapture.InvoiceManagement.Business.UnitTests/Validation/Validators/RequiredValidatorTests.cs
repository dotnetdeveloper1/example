using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RequiredValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new RequiredValidator();
        }

        [TestMethod]
        public void Validate_WhenFieldNameIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new Annotation(), null));
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsNull_ShouldReturnFailedResult()
        {
            var result = target.Validate(null, testFieldName);
            
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

        private RequiredValidator target;
        private readonly string validationResultMesage = $"Missing annotation for {testFieldName}.";
        private const string testFieldType = "testField";
        private const string testFieldName = "testFieldName";
        private const string testFieldValue = "some data";
    }
}
