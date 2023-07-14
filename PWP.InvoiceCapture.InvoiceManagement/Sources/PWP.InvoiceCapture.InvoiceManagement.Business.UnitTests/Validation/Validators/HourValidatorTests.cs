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
    public class HourValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new HourValidator();
        }

        [TestMethod]
        public void Validate_WhenEntityIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(null, cultureInfo, testFieldName));
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
                target.Validate(new Annotation() { FieldValue = null}, cultureInfo, testFieldName));
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

        [DataRow("10:11:12:13")]
        [DataRow("-10:11")]
        [DataRow("10:60")]
        [DataRow("10:10:60")]
        [DataRow("-10:-11")]
        [DataRow("11:62")]
        [DataRow("11:10:")]
        [DataRow("11:10::")]
        [DataRow("11::")]
        [DataRow(":10:10:")]
        [DataRow(":11")]
        [DataRow("11:11d")]
        [DataRow("hh:11")]
        [DataRow("1h:1m")]
        [TestMethod]
        public void Validate_WhenFieldValueIsWrong_ShouldReturnFailedResult(string hours)
        {
            var result =  target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = hours,
            },
            cultureInfo,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        [DataRow("10:11")]
        [DataRow("10:11:12")]
        [DataRow("01:11:12")]
        [DataRow("01:02:03")]
        [DataRow("1:2:3")]
        [TestMethod]
        public void Validate_WhenFieldValueIsCorrectHour_ShouldReturnValidationResultOk(string hours)
        {
            var result =  target.Validate(new Annotation()
            {
                FieldType = testFieldType,
                FieldValue = hours,
            }, 
            cultureInfo,
            testFieldName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        private HourValidator target;
        private readonly string validationResultMesage = $"Field {testFieldName} can't be converted to hour.";
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        private const string testFieldType = "testField";
        private const string testFieldName = "testFieldName";
    }
}
