using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
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
    public class TotalMultiplicationValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new TotalMultiplicationValidator();
        }

        [TestMethod]
        public void Validate_WhenAnnotationsToMultiplyAreNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(null, new Annotation(), cultureInfo, fields));
        }

        [TestMethod]
        public void Validate_WhenTotalAnnotationIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new[] { new Annotation() }, null, cultureInfo, fields));
        }

        [TestMethod]
        public void Validate_WhenFieldNamesListIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new[] { new Annotation() { FieldValue = "asdasd" } }, new Annotation(), cultureInfo, null));
        }

        [TestMethod]
        public void Validate_WhenMultiplicationIsIncorrect_ShouldReturnFailedResult()
        {
            var annotations = new[]
            {
                new Annotation() { FieldValue = "0.32" ,FieldType = testFieldType },
                new Annotation() { FieldValue = "1.56" , FieldType = testFieldType2 }
            };

            var totalAnnotation = new Annotation() { FieldValue = "2.23" };
            var result = target.Validate(annotations, totalAnnotation, cultureInfo, fields);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual($"Total is not equal to multiplication of {testFieldName},{testFieldName2}.", result.Message);
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void Validate_WhenLessThenTwoItemsToMultiply_ShouldReturnOkResult(int count)
        {
            var annotations = Enumerable
                .Range(0, count)
                .Select((index) => new Annotation())
                .ToArray();

            var totalAnnotation = new Annotation();
            var result = target.Validate(annotations, totalAnnotation, cultureInfo, fields);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_WhenOneAnnotationIsNullAndCountIsTwo_ShouldReturnOkResult()
        {
            var annotations = new[]
            {
                new Annotation() { FieldValue = "0.32" ,FieldType = testFieldType },
                null
            };

            var totalAnnotation = new Annotation();
            var result = target.Validate(annotations, totalAnnotation, cultureInfo, fields);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_WhenMultiplicationIsCorrect_ShouldReturnOkResult()
        {
            var annotations = new[]
            {
                new Annotation() { FieldValue = "0.32" ,FieldType = testFieldType },
                new Annotation() { FieldValue = "1.56" , FieldType = testFieldType2 }
            };

            var totalAnnotation = new Annotation() { FieldValue = "0.5" };
            var result = target.Validate(annotations, totalAnnotation, cultureInfo, fields);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [DataRow("1", "1:00", "1")]
        [DataRow("2", "1:00", "2")]
        [DataRow("2.5", "1:00", "2.5")]
        [DataRow("2.5", "1:30", "3.75")]
        [DataRow("2.5", "2:32", "6.33")]
        [DataRow("2.5","2:32:60","6.38")]
        [TestMethod]
        public void Validate_WhenMultiplicationIsCorrectAndContainsHours_ShouldReturnOkResult(string price, string hours, string total)
        {
            var annotations = new[]
            {
                new Annotation() { FieldValue = price, FieldType = testFieldType },
                new Annotation() { FieldValue = hours, FieldType = testFieldType2 }
            };

            var totalAnnotation = new Annotation() { FieldValue = total };
            var result = target.Validate(annotations, totalAnnotation, cultureInfo, fields);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        [DataRow("2.5", "1:30", "4")]
        [TestMethod]
        public void Validate_WhenMultiplicationIsNotCorrectAndContainsHours_ShouldReturnFailedResult(string price, string hours, string total)
        {
            var annotations = new[]
            {
                new Annotation() { FieldValue = price, FieldType = testFieldType },
                new Annotation() { FieldValue = hours, FieldType = testFieldType2 }
            };

            var totalAnnotation = new Annotation() { FieldValue = total };
            var result = target.Validate(annotations, totalAnnotation, cultureInfo, fields);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual($"Total is not equal to multiplication of {testFieldName},{testFieldName2}.", result.Message);
        }

        [TestMethod]
        public void Validate_MultiplicationForLongDecimals_ShouldReturnOkResult()
        {
            var annotations = new[]
            {
                new Annotation() { FieldValue = "0.009200" , FieldType = testFieldType },
                new Annotation() { FieldValue = "655" , FieldType = testFieldType2 }
            };

            var totalAnnotation = new Annotation() { FieldValue = "6.03" };
            var result = target.Validate(annotations, totalAnnotation, cultureInfo, fields);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        private TotalMultiplicationValidator target;
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;

        private readonly Dictionary<string, string> fields = new List<Field>()
        {
            new Field() { DisplayName = testFieldName,  Id = Convert.ToInt32(testFieldType) },
            new Field() { DisplayName = testFieldName2, Id = Convert.ToInt32(testFieldType2) }
        }
        .ToDictionary(field => field.Id.ToString(), field => field.DisplayName);

        private const string testFieldType = "1";
        private const string testFieldType2 = "2";
        private const string testFieldName = "testFieldName";
        private const string testFieldName2 = "testFieldName2";
    }
}
