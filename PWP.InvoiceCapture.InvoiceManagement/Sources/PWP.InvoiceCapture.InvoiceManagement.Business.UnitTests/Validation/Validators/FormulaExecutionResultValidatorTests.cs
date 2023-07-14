using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FormulaExecutionResultValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            formulaExtractionServiceMock = mockRepository.Create<IFormulaExtractionService>();
            target = new FormulaExecutionResultValidator(formulaExtractionServiceMock.Object);
        }

        [TestMethod]
        public void Instance_WhenFormulaExtractionServiceIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FormulaExecutionResultValidator(null));
        }

        [TestMethod]
        public void Validate_WhenAnnotationsIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                target.Validate(null, annotations[5], fieldName, formula));
        }

        [TestMethod]
        public void Validate_WhenResultAnnotationIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(annotations, null, fieldName, formula));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Validate_WhenFormulaIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string formula) 
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(annotations, annotations[5], fieldName, formula));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Validate_WhenResultFieldNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string fieldName) 
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(annotations, annotations[5], fieldName, formula));
        }

        [TestMethod]
        [DataRow(3, "[1]+[2]", new[] { 1, 2 }, "1+2")]
        [DataRow(4, "[1]+[2]+1", new[] { 1, 2 }, "1+2+1")]
        [DataRow(5, "[1]*[6]-1", new[] { 1, 6 }, "1*6-1")]
        [DataRow(5, "[1]*[6]-1+[100]", new[] { 1, 6, 100 }, "1*6-1")]
        [DataRow(6, "([2]+[7])*[3]-([12]+[9])", new[] { 2, 7, 3, 12, 9 }, "(2+7)*3-(12+9)")]
        public void Validate_WhenResultAnnotationIsEqualToExecutionResult_ShouldReturnOk(int resultIndex, string formula, int[] fieldIds, string normalizedFormula) 
        {
            var actualKeyValues = new List<Dictionary<int, decimal>>();

            formulaExtractionServiceMock
                .Setup(extractionService => extractionService.GetFieldIds(formula))
                .Returns(fieldIds.ToList());

            formulaExtractionServiceMock
                .Setup(extractionService => extractionService.GetNormalizedFormula(formula, Capture.In(actualKeyValues)))
                .Returns(normalizedFormula);

            var expectedKeyValues = fieldIds.ToDictionary(
                id => id, 
                id => id <= annotations.Count ? decimal.Parse(annotations[id - 1].FieldValue) : 0);

            var validationResult = target.Validate(annotations, annotations[resultIndex - 1], fieldName, formula);
            
            Assert.IsTrue(validationResult.IsValid);
            Assert.AreEqual(1, actualKeyValues.Count);

            CollectionAssert.AreEqual(expectedKeyValues, actualKeyValues[0]);
        }

        [TestMethod]
        public void Validate_WhenResultAnnotationIsNotEqualToExecutionResult_ShouldReturnFailed() 
        {
            var fieldIds = new List<int> { 1, 2 };
            var invalidNormalizedFormula = "1+1000";

            formulaExtractionServiceMock
                .Setup(extractionService => extractionService.GetFieldIds(formula))
                .Returns(fieldIds);

            formulaExtractionServiceMock
                .Setup(extractionService => extractionService.GetNormalizedFormula(formula, It.IsAny<Dictionary<int, decimal>>()))
                .Returns(invalidNormalizedFormula);

            var validationResult = target.Validate(annotations, annotations[1], fieldName, formula);

            Assert.IsFalse(validationResult.IsValid);
        }

        private FormulaExecutionResultValidator target;
        private MockRepository mockRepository;
        private Mock<IFormulaExtractionService> formulaExtractionServiceMock;
        
        private const string formula = "[1]+[4]-[3]";
        private const string fieldName = "ResultFieldName";

        private readonly List<Annotation> annotations = new List<Annotation>
        {
            new Annotation { FieldType = "1", FieldValue = "1" },
            new Annotation { FieldType = "2", FieldValue = "2" },
            new Annotation { FieldType = "3", FieldValue = "3" },
            new Annotation { FieldType = "4", FieldValue = "4" },
            new Annotation { FieldType = "5", FieldValue = "5" },
            new Annotation { FieldType = "6", FieldValue = "6" },
            new Annotation { FieldType = "7", FieldValue = "7" },
            new Annotation { FieldType = "8", FieldValue = "8" },
            new Annotation { FieldType = "9", FieldValue = "9" },
            new Annotation { FieldType = "10", FieldValue = "10" },
            new Annotation { FieldType = "11", FieldValue = "11" },
            new Annotation { FieldType = "12", FieldValue = "12" },
        };
    }
}
