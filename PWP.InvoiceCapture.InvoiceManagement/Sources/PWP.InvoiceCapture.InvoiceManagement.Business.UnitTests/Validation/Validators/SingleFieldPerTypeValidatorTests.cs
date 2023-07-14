using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SingleFieldPerTypeValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new SingleFieldPerTypeValidator();
        }

        [TestMethod]
        public void Validate_WhenEntityIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(null, fields));
        }

        [TestMethod]
        public void Validate_WhenFieldNamesListIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(new List<Annotation>() { new Annotation() { FieldValue = "asdasd" } }, null));
        }

        [TestMethod]
        public void Validate_WhenThereAreMultipleFieldTypesInList_ShouldReturnFailedResult()
        {
            var annotations = new List<Annotation>()
            {
                new Annotation() { FieldType = testFieldType },
                new Annotation() { FieldType = testFieldType }
            };
            var result = target.Validate(annotations, fields);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(validationResultMesage, result.Message);
        }

        [TestMethod]
        public void Validate_WhenThereIsSingleFieldTypesInList_ShouldReturnOkResult()
        {            
            var result = target.Validate(annotations, fields);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        private SingleFieldPerTypeValidator target;
        private readonly string validationResultMesage = $"Field type {testFieldName} is present more than once.";
        private readonly List<Annotation> annotations = new List<Annotation>()
        {
            new Annotation() { FieldType = testFieldType },
            new Annotation() { FieldType = testFieldType2 }
        };

        private readonly Dictionary<string, string> fields = new List<Field>()
        {
            new Field() { DisplayName = testFieldName, Id = Convert.ToInt32(testFieldType) },
            new Field() { DisplayName = testFieldName2, Id = Convert.ToInt32(testFieldType2) }
        }.ToDictionary(field => field.Id.ToString(), field => field.DisplayName);

        private const string testFieldType = "1";
        private const string testFieldType2 = "2";
        private const string testFieldName = "testFieldName";
        private const string testFieldName2 = "testFieldName2";
    }
}
