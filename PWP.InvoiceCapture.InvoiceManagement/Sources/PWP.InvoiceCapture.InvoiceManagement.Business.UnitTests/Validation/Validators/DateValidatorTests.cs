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
    public class DateValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new DateValidator();
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
        [DataRow("    ")]
        public void Validate_WhenFieldNameIsEmpty_ShouldThrowArgumentNullException(string fieldName)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new Annotation() { FieldValue = "asdasd" }, cultureInfo, fieldName));
        }

        [TestMethod]
        public void Validate_WhenEntityValueIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new Annotation() { FieldValue = null}, cultureInfo, testFieldName));
        }


        [TestMethod]
        public void Validate_WhenFieldValueNotDate_ShouldReturnFailedResult()
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = invalidDate,
            }, 
            cultureInfo,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        [TestMethod]
        public  void Validate_WhenFieldValueIsCorrectDate_ShouldReturnValidationResultOk()
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = validDate,
            }, 
            cultureInfo,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsCorrectFrenchDate_ShouldReturnValidationResultOk()
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = validFrenchDate,
            }, 
            frenchCulture,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsCorrectUsDate_ShouldReturnValidationResultOk()
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = validUsDate,
            }, 
            uSCulture,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsNorFrenchDate_ShouldReturnFailedResult()
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = validUsDate,
            }, 
            frenchCulture,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsLessThanMinSqlDate_ShouldReturnFailedResult()
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = lessThenMinDate,
            },
            uSCulture,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(dateRangeValidationResultMesage, result.Message);
        }

        [TestMethod]
        public void Validate_WhenFieldValueIsMoreThenMaxDate_ShouldReturnFailedResult()
        {
            var result = target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = moreThenMaxDate,
            }, 
            uSCulture,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        private DateValidator target;
        private readonly string validationResultMesage = $"Field {testFieldName} can't be converted to date.";
        private readonly string dateRangeValidationResultMesage = $"Field {testFieldName} must be between 1/1/1753 and 12/31/9999.";
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        
        private readonly CultureInfo frenchCulture = new CultureInfo("fr-FR");
        private readonly CultureInfo uSCulture = new CultureInfo("en-US");
       
        private const string testFieldType = "testField";
        private const string testFieldName = "testFieldName";
        private const string validDate = "2020-06-07 22:11:15";
        private const string validFrenchDate = "25/11/2011";
        private const string validUsDate = "01/25/2011";
        private const string invalidDate = "2020dt";
        private const string lessThenMinDate = "01/01/1752";
        private const string moreThenMaxDate = "01/01/10000";

    }
}
