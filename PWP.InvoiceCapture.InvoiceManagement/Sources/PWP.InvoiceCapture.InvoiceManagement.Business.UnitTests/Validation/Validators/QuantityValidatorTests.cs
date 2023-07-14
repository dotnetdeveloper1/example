using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class QuantityValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            hourValidatorMock = mockRepository.Create<IHourValidator>();
            decimalValidatorMock = mockRepository.Create<IDecimalValidator>();

            target = new QuantityValidator(hourValidatorMock.Object, decimalValidatorMock.Object);
        }

        [TestMethod]
        public void Instance_WhenHourValidatorMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new QuantityValidator(null, decimalValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenDecimalValidatorMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new QuantityValidator(hourValidatorMock.Object, null));
        }

        [TestMethod]
        public void Validate_WhenContainsHourSeparator_ShouldUseHourValidator()
        {
            var dataAnnotation = new Annotation()
            {
                FieldValue = hourValue
            };

            hourValidatorMock
                .Setup(hourValidator => hourValidator.Validate(dataAnnotation, cultureInfo, fieldName))
                .Returns(ValidationResult.Ok);
            var result = target.Validate(dataAnnotation, cultureInfo, fieldName);
        }

        [TestMethod]
        public void Validate_WhenNotContainsHourSeparator_ShouldUseDecimalValidator()
        {
            var dataAnnotation = new Annotation()
            {
                FieldValue = decimalValue
            };

            decimalValidatorMock
                .Setup(decimalValidator => decimalValidator.Validate(dataAnnotation, cultureInfo, fieldName))
                .Returns(ValidationResult.Ok);
            var result = target.Validate(dataAnnotation, cultureInfo, fieldName);
        }

        private QuantityValidator target;

        private MockRepository mockRepository;
        private Mock<IHourValidator> hourValidatorMock;
        private Mock<IDecimalValidator> decimalValidatorMock;
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;


        private const string hourValue = "10:11"; 
        private const string decimalValue = "10.11";
        private const string fieldName = "fieldName";
    }
}
