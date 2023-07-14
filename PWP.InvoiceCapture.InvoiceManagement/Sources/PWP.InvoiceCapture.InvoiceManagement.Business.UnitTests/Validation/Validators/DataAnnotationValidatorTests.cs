using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DataAnnotationValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            requiredValidatorMock = mockRepository.Create<IRequiredValidator>();
            requiredValueValidatorMock = mockRepository.Create<IRequiredValueValidator>();
            maxLengthValidatorMock = mockRepository.Create<IMaxLengthValidator>();
            minLengthValidatorMock = mockRepository.Create<IMinLengthValidator>();
            maxValueValidatorMock = mockRepository.Create<IMaxValueValidator>();
            minValueValidatorMock = mockRepository.Create<IMinValueValidator>();
            dateValidatorMock = mockRepository.Create<IDateValidator>();
            decimalValidatorMock = mockRepository.Create<IDecimalValidator>();
            quantityValidatorMock = mockRepository.Create<IQuantityValidator>();
            singleFieldPerTypeValidatorMock = mockRepository.Create<ISingleFieldPerTypeValidator>();
            formulaExecutionResultValidatorMock = mockRepository.Create<IFormulaExecutionResultValidator>();
            lineOrderValidatorMock = mockRepository.Create<ILineOrderValidator>();
            totalMultiplicationValidatorMock = mockRepository.Create<ITotalMultiplicationValidator>();

            target = new DataAnnotationValidator(
                requiredValidatorMock.Object,
                requiredValueValidatorMock.Object,
                dateValidatorMock.Object,
                decimalValidatorMock.Object,
                quantityValidatorMock.Object,
                singleFieldPerTypeValidatorMock.Object,
                lineOrderValidatorMock.Object,
                totalMultiplicationValidatorMock.Object,
                formulaExecutionResultValidatorMock.Object,
                maxLengthValidatorMock.Object,
                minLengthValidatorMock.Object,
                maxValueValidatorMock.Object,
                minValueValidatorMock.Object);
        }

        [TestMethod]
        public void Instance_WhenRequiredValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(null, requiredValueValidatorMock.Object, dateValidatorMock.Object, decimalValidatorMock.Object,
                    quantityValidatorMock.Object, singleFieldPerTypeValidatorMock.Object, 
                    lineOrderValidatorMock.Object, totalMultiplicationValidatorMock.Object,
                    formulaExecutionResultValidatorMock.Object,
                    maxLengthValidatorMock.Object, minLengthValidatorMock.Object,
                    maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenRequiredValueValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(requiredValidatorMock.Object, null, 
                    dateValidatorMock.Object, decimalValidatorMock.Object,
                    quantityValidatorMock.Object, singleFieldPerTypeValidatorMock.Object, 
                    lineOrderValidatorMock.Object, totalMultiplicationValidatorMock.Object,
                    formulaExecutionResultValidatorMock.Object,
                    maxLengthValidatorMock.Object, minLengthValidatorMock.Object,
                    maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenMaxLengthValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(requiredValidatorMock.Object, requiredValueValidatorMock.Object,
                    dateValidatorMock.Object, decimalValidatorMock.Object, quantityValidatorMock.Object,
                    singleFieldPerTypeValidatorMock.Object,
                    lineOrderValidatorMock.Object, totalMultiplicationValidatorMock.Object,
                    formulaExecutionResultValidatorMock.Object,
                    null, minLengthValidatorMock.Object,
                    maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenDateValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(requiredValidatorMock.Object, requiredValueValidatorMock.Object, 
                    null, decimalValidatorMock.Object,
                    quantityValidatorMock.Object, singleFieldPerTypeValidatorMock.Object,
                    lineOrderValidatorMock.Object, totalMultiplicationValidatorMock.Object,
                    formulaExecutionResultValidatorMock.Object,
                    maxLengthValidatorMock.Object, minLengthValidatorMock.Object,
                    maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenDecimalValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(requiredValidatorMock.Object, requiredValueValidatorMock.Object,
                dateValidatorMock.Object, null,
                quantityValidatorMock.Object, singleFieldPerTypeValidatorMock.Object,
                lineOrderValidatorMock.Object, totalMultiplicationValidatorMock.Object,
                formulaExecutionResultValidatorMock.Object, maxLengthValidatorMock.Object,
                minLengthValidatorMock.Object, maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenFormulaExecutionResultValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(requiredValidatorMock.Object, requiredValueValidatorMock.Object, 
                dateValidatorMock.Object, decimalValidatorMock.Object,
                quantityValidatorMock.Object, singleFieldPerTypeValidatorMock.Object,
                lineOrderValidatorMock.Object, totalMultiplicationValidatorMock.Object, null,
                maxLengthValidatorMock.Object, minLengthValidatorMock.Object,
                maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenSingleFieldPerTypeValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(requiredValidatorMock.Object, requiredValueValidatorMock.Object,
                dateValidatorMock.Object, decimalValidatorMock.Object,
                quantityValidatorMock.Object, null,
                lineOrderValidatorMock.Object, totalMultiplicationValidatorMock.Object,
                formulaExecutionResultValidatorMock.Object,
                maxLengthValidatorMock.Object, minLengthValidatorMock.Object,
                maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenLineOrderValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(requiredValidatorMock.Object, requiredValueValidatorMock.Object,
                dateValidatorMock.Object, decimalValidatorMock.Object,
                quantityValidatorMock.Object, singleFieldPerTypeValidatorMock.Object,
                null, totalMultiplicationValidatorMock.Object,
                formulaExecutionResultValidatorMock.Object, maxLengthValidatorMock.Object, minLengthValidatorMock.Object,
                maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTotalMultiplicationValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(requiredValidatorMock.Object, requiredValueValidatorMock.Object,
                dateValidatorMock.Object, decimalValidatorMock.Object,
                quantityValidatorMock.Object, singleFieldPerTypeValidatorMock.Object,
                lineOrderValidatorMock.Object, null, formulaExecutionResultValidatorMock.Object,
                maxLengthValidatorMock.Object, minLengthValidatorMock.Object,
                maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenQuantityValidatorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DataAnnotationValidator(requiredValidatorMock.Object, requiredValueValidatorMock.Object,
                dateValidatorMock.Object, decimalValidatorMock.Object,
                null, singleFieldPerTypeValidatorMock.Object,
                lineOrderValidatorMock.Object, totalMultiplicationValidatorMock.Object,
                formulaExecutionResultValidatorMock.Object,
                maxLengthValidatorMock.Object, minLengthValidatorMock.Object,
                maxValueValidatorMock.Object, minValueValidatorMock.Object));
        }

        [TestMethod]
        public void Validate_WhenDataAnnotationIsCorrect_ShouldReturnIsValidResult()
        {
            var invoice = new Invoice { Status = InvoiceStatus.PendingReview };
            var invoiceLines = new List<InvoiceLine>() { new InvoiceLine() };

            var dataAnnotation = new DataAnnotation()
            {
                InvoiceAnnotations = annotations,
                InvoiceLineAnnotations = lineAnnotations
            };

            var actualRequiredAnnotations = new List<Annotation>();
            var actualRequiredValueAnnotations = new List<Annotation>();
            var actualMaxLengthAnnotations = new List<Annotation>();
            var actualMinLengthAnnotations = new List<Annotation>();
            var actualMaxValueAnnotations = new List<Annotation>();
            var actualMinValueAnnotations = new List<Annotation>();
            var actualDecimalAnnotations = new List<Annotation>();
            var actualQuantitiesAnnotations = new List<Annotation>();
            var actualDateAnnotations = new List<Annotation>();
            var actualResultAnnotation = new List<Annotation>();
            var actualSingleFieldPerType = new List<List<Annotation>>();
            var actualLineOrderAnnotations = new List<List<LineAnnotation>>();
            var actualFieldsToMultiply = new List<Annotation[]>();
            var actualTotalMultiplication = new List<Annotation>();

            requiredValidatorMock
                .Setup(validator => validator.Validate(Capture.In(actualRequiredAnnotations), It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            requiredValueValidatorMock
                .Setup(validator => validator.Validate(Capture.In(actualRequiredValueAnnotations), It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            maxLengthValidatorMock
                .Setup(validator => validator.Validate(Capture.In(actualMaxLengthAnnotations), maxLength, It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            minLengthValidatorMock
               .Setup(validator => validator.Validate(Capture.In(actualMinLengthAnnotations), minLength, It.IsAny<string>()))
               .Returns(ValidationResult.Ok);

            maxValueValidatorMock
               .Setup(validator => validator.Validate(Capture.In(actualMaxValueAnnotations), maxValue, It.IsAny<string>()))
               .Returns(ValidationResult.Ok);

            minValueValidatorMock
               .Setup(validator => validator.Validate(Capture.In(actualMinValueAnnotations), minValue, It.IsAny<string>()))
               .Returns(ValidationResult.Ok);

            decimalValidatorMock
                .Setup(validator => validator.Validate(Capture.In(actualDecimalAnnotations), cultureInfo, It.IsAny<string>()))
                .Returns(ValidationResult.Ok);
            
            quantityValidatorMock
                .Setup(validator => validator.Validate(Capture.In(actualQuantitiesAnnotations), cultureInfo, It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            dateValidatorMock
                .Setup(validator => validator.Validate(Capture.In(actualDateAnnotations), cultureInfo, It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            formulaExecutionResultValidatorMock
                .Setup(validator => validator.Validate(annotations, Capture.In(actualResultAnnotation), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            singleFieldPerTypeValidatorMock
                .Setup(validator => validator.Validate(Capture.In(actualSingleFieldPerType), It.IsAny< Dictionary<string, string>> ()))
                .Returns(ValidationResult.Ok);

            lineOrderValidatorMock
                .Setup(lineOrderValidator => lineOrderValidator.Validate(Capture.In(actualLineOrderAnnotations)))
                .Returns(ValidationResult.Ok);

            totalMultiplicationValidatorMock
                .Setup(totalMultiplicationValidator => totalMultiplicationValidator.Validate(Capture.In(actualFieldsToMultiply), Capture.In(actualTotalMultiplication), cultureInfo, It.IsAny<Dictionary<string, string>>()))
                .Returns(ValidationResult.Ok);

            var actualResult = target.Validate(dataAnnotation, cultureInfo, fields);

            Assert.IsNotNull(actualResult);
            Assert.IsTrue(actualResult.IsValid);
            Assert.AreEqual(InvoiceLineFieldTypes.Total, actualTotalMultiplication[0].FieldType);

            CollectionAssert.AreEquivalent(expectedRequired, actualRequiredAnnotations.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedRequiredValues, actualRequiredValueAnnotations.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedDecimals, actualDecimalAnnotations.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedQuantities, actualQuantitiesAnnotations.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedDates, actualDateAnnotations.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedMaxLength, actualMaxLengthAnnotations.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedMinLength, actualMinLengthAnnotations.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedMaxValue, actualMaxValueAnnotations.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedMinValue, actualMinValueAnnotations.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedResultAnnotations, actualResultAnnotation.Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(expectedFieldsToMultiply, actualFieldsToMultiply[0].Select(annotation => annotation.FieldType).ToList());
            CollectionAssert.AreEquivalent(lineAnnotations, actualLineOrderAnnotations[0]);

            var invoiceAnnotationsFiledTypes = dataAnnotation.InvoiceAnnotations.Select(annotation => annotation.FieldType);
            var lineAnnotationsFieldTypes = dataAnnotation.InvoiceLineAnnotations.SelectMany(item => item.LineItemAnnotations).Select(item => item.FieldType);
            var expectedSingleFieldPerType = invoiceAnnotationsFiledTypes.Concat(lineAnnotationsFieldTypes).ToList();

            CollectionAssert.AreEquivalent(expectedSingleFieldPerType, actualSingleFieldPerType.SelectMany(annotations => annotations).Select(annotation => annotation.FieldType).ToList());
        }

        [TestMethod]
        public void Validate_WhenDataAnnotationIsCorrectButFieldTypeIsWrong_ShouldReturnIsInvalidResult()
        {
            var invoice = new Invoice { Status = InvoiceStatus.PendingReview };
            var invoiceLines = new List<InvoiceLine>() { new InvoiceLine() };

            var dataAnnotation = new DataAnnotation()
            {
                InvoiceAnnotations = annotations,
                InvoiceLineAnnotations = lineAnnotations
            };

            var actualRequiredAnnotations = new List<Annotation>();
            var actualRequiredValueAnnotations = new List<Annotation>();
            var actualMaxLengthAnnotations = new List<Annotation>();
            var actualMinLengthAnnotations = new List<Annotation>();
            var actualMaxValueAnnotations = new List<Annotation>();
            var actualMinValueAnnotations = new List<Annotation>();
            var actualDecimalAnnotations = new List<Annotation>();
            var actualQuantitiesAnnotations = new List<Annotation>();
            var actualDateAnnotations = new List<Annotation>();
            var actualResultAnnotation = new List<Annotation>();
            var actualSingleFieldPerType = new List<List<Annotation>>();
            var actualLineOrderAnnotations = new List<List<LineAnnotation>>();
            var actualFieldsToMultiply = new List<Annotation[]>();
            var actualTotalMultiplication = new List<Annotation>();

            requiredValidatorMock
                .Setup(requiredField => requiredField.Validate(Capture.In(actualRequiredAnnotations), It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            requiredValueValidatorMock
                .Setup(requiredField => requiredField.Validate(Capture.In(actualRequiredValueAnnotations), It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            maxLengthValidatorMock
                .Setup(requiredField => requiredField.Validate(Capture.In(actualMaxLengthAnnotations), maxLength, It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            minLengthValidatorMock
               .Setup(requiredField => requiredField.Validate(Capture.In(actualMinLengthAnnotations), minLength, It.IsAny<string>()))
               .Returns(ValidationResult.Ok);

            maxValueValidatorMock
               .Setup(requiredField => requiredField.Validate(Capture.In(actualMaxValueAnnotations), maxValue, It.IsAny<string>()))
               .Returns(ValidationResult.Ok);

            minValueValidatorMock
               .Setup(requiredField => requiredField.Validate(Capture.In(actualMinValueAnnotations), minValue, It.IsAny<string>()))
               .Returns(ValidationResult.Ok);

            decimalValidatorMock
                .Setup(requiredField => requiredField.Validate(Capture.In(actualDecimalAnnotations), cultureInfo, It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            quantityValidatorMock
                .Setup(requiredField => requiredField.Validate(Capture.In(actualQuantitiesAnnotations), cultureInfo, It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            dateValidatorMock
                .Setup(requiredField => requiredField.Validate(Capture.In(actualDateAnnotations), cultureInfo, It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            formulaExecutionResultValidatorMock
                .Setup(validator => validator.Validate(annotations, Capture.In(actualResultAnnotation), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ValidationResult.Ok);

            singleFieldPerTypeValidatorMock
                .Setup(requiredField => requiredField.Validate(Capture.In(actualSingleFieldPerType), It.IsAny<Dictionary<string, string>>()))
                .Returns(ValidationResult.Ok);

            lineOrderValidatorMock
                .Setup(lineOrderValidator => lineOrderValidator.Validate(Capture.In(actualLineOrderAnnotations)))
                .Returns(ValidationResult.Ok);

            totalMultiplicationValidatorMock
                .Setup(totalMultiplicationValidator => totalMultiplicationValidator.Validate(Capture.In(actualFieldsToMultiply), Capture.In(actualTotalMultiplication), cultureInfo, It.IsAny<Dictionary<string, string>>()))
                .Returns(ValidationResult.Ok);

            var actualResult = target.Validate(dataAnnotation, cultureInfo, wrongFields);

            Assert.IsNotNull(actualResult);
            Assert.IsFalse(actualResult.IsValid);
            Assert.AreEqual(actualResult.Message, validationResultMesage);
        }

        private readonly List<Annotation> annotations = new List<Annotation>()
        {
            new Annotation { FieldType = FieldTypes.VendorName.ToString(), FieldValue = "name" },
            new Annotation { FieldType = FieldTypes.VendorAddress.ToString(), FieldValue = "address" },
            new Annotation { FieldType = FieldTypes.TaxNumber.ToString(), FieldValue = "taxN" },
            new Annotation { FieldType = FieldTypes.VendorPhone.ToString(), FieldValue = "phone" },
            new Annotation { FieldType = FieldTypes.VendorEmail.ToString(), FieldValue = "email" },
            new Annotation { FieldType = FieldTypes.VendorWebsite.ToString(), FieldValue = "website" },
            new Annotation { FieldType = FieldTypes.InvoiceDate.ToString(), FieldValue = "2020-01-01" },
            new Annotation { FieldType = FieldTypes.DueDate.ToString(), FieldValue = "2020-01-02" },
            new Annotation { FieldType = FieldTypes.PoNumber.ToString(), FieldValue = "poN" },
            new Annotation { FieldType = FieldTypes.InvoiceNumber.ToString(), FieldValue = "invN1" },
            new Annotation { FieldType = FieldTypes.TaxAmount.ToString(), FieldValue = "1.2" },
            new Annotation { FieldType = FieldTypes.FreightAmount.ToString(), FieldValue = "1.1" },
            new Annotation { FieldType = FieldTypes.SubTotal.ToString(), FieldValue = "2" },
            new Annotation { FieldType = FieldTypes.Total.ToString(), FieldValue = "4.3" }
        };

        private readonly List<LineAnnotation> lineAnnotations = new List<LineAnnotation>()
        {
            new LineAnnotation
            {
                OrderNumber = 1,
                LineItemAnnotations = new List<Annotation>
                {
                    new Annotation { FieldType = InvoiceLineFieldTypes.Description, FieldValue = description },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Number, FieldValue = number },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Price, FieldValue = price.ToString() },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Quantity, FieldValue = quantity.ToString() },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Total, FieldValue = total.ToString() }
                }
            }
        };

        private readonly List<string> expectedRequired = new List<string>() { FieldTypes.InvoiceNumber.ToString(), InvoiceLineFieldTypes.Total };
        private readonly List<string> expectedRequiredValues = new List<string>() { FieldTypes.InvoiceNumber.ToString(), InvoiceLineFieldTypes.Total, InvoiceLineFieldTypes.Price, InvoiceLineFieldTypes.Quantity };
        private readonly List<string> expectedMaxLength = new List<string>() { FieldTypes.InvoiceNumber.ToString() };
        private readonly List<string> expectedMinLength = new List<string>() { FieldTypes.InvoiceNumber.ToString() };
        private readonly List<string> expectedMaxValue = new List<string>() { FieldTypes.PoNumber.ToString() };
        private readonly List<string> expectedMinValue = new List<string>() { FieldTypes.PoNumber.ToString() };
        private readonly List<string> expectedDecimals = new List<string>() { FieldTypes.Total.ToString(), FieldTypes.PoNumber.ToString(), InvoiceLineFieldTypes.Total, InvoiceLineFieldTypes.Price };
        private readonly List<string> expectedQuantities = new List<string>() { InvoiceLineFieldTypes.Quantity };
        private readonly List<string> expectedDates = new List<string>() { FieldTypes.InvoiceDate.ToString().ToString(), FieldTypes.DueDate.ToString() };
        private readonly List<string> expectedResultAnnotations = new List<string>() { FieldTypes.Total.ToString() };
        private readonly List<string> expectedFieldsToMultiply = new List<string>() { InvoiceLineFieldTypes.Price, InvoiceLineFieldTypes.Quantity };
        private readonly List<Field> fields = new List<Field>()
        {
            new Field() { Id = FieldTypes.InvoiceNumber, Type = FieldType.String, IsRequired = true, MinLength = minLength, MaxLength = maxLength },
            new Field() { Id = FieldTypes.PoNumber, Type = FieldType.Decimal, MinValue = minValue, MaxValue = maxValue },
            new Field() { Id = FieldTypes.InvoiceDate, Type = FieldType.DateTime },
            new Field() { Id = FieldTypes.DueDate, Type = FieldType.DateTime },
            new Field() { Id = FieldTypes.Total, Type = FieldType.Decimal, Formula = "[11]+[12]+[13]" }
        };

        private readonly List<Field> wrongFields = new List<Field>()
        {
            new Field() { Id = FieldTypes.InvoiceNumber, Type = (FieldType)wrongFieldType, IsRequired = true, MinLength = minLength, MaxLength = maxLength }
        };

        private DataAnnotationValidator target;

        private MockRepository mockRepository;
        private Mock<IRequiredValidator> requiredValidatorMock;
        private Mock<IRequiredValueValidator> requiredValueValidatorMock;
        private Mock<IMaxLengthValidator> maxLengthValidatorMock;
        private Mock<IMinLengthValidator> minLengthValidatorMock;
        private Mock<IMaxValueValidator> maxValueValidatorMock;
        private Mock<IMinValueValidator> minValueValidatorMock;
        private Mock<IDecimalValidator> decimalValidatorMock;
        private Mock<IQuantityValidator> quantityValidatorMock;
        private Mock<IDateValidator> dateValidatorMock;
        private Mock<ISingleFieldPerTypeValidator> singleFieldPerTypeValidatorMock;
        private Mock<IFormulaExecutionResultValidator> formulaExecutionResultValidatorMock;
        private Mock<ILineOrderValidator> lineOrderValidatorMock;
        private Mock<ITotalMultiplicationValidator> totalMultiplicationValidatorMock;
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;


        private const int maxLength = 250;
        private const int minLength = 0;
        private const int maxValue= 250;
        private const int minValue = 0;
        private const string description = "Description";
        private const string number = "Number";
        private const decimal price = 1.1m;
        private const decimal quantity = 10;
        private const decimal total = 11;
        private const int wrongFieldType = 9999;
        private readonly string validationResultMesage = $"Unknown field type {wrongFieldType}";
    }
}
