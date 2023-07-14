using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Validation.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class LineOrderValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new LineOrderValidator();
        }

        [TestMethod]
        public void Validate_WhenLineAnnotationIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                target.Validate(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void Validate_WhenOrderNumberIsEqualOrLessThenZero_ShouldReturnFailedResult(int orderNumber)
        {
            var annotations = new List<LineAnnotation>
            {
                new LineAnnotation { OrderNumber = orderNumber }
            };

            var result = target.Validate(annotations);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("OrderNumber should be greater then 0.", result.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(129)]
        public void Validate_WhenOrderNumberIsNotUnique_ShouldReturnFailedResult(int orderNumber)
        {
            var annotations = new List<LineAnnotation>
            {
                new LineAnnotation { OrderNumber = orderNumber },
                new LineAnnotation { OrderNumber = orderNumber }
            };

            var result = target.Validate(annotations);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("InvoiceLineAnnotations contains OrderNumbers with the same values.", result.Message);
        }

        [TestMethod]
        public void Validate_WhenLineAnnotationsAreCorrect_ShouldReturnOk()
        {
            var annotations = new List<LineAnnotation>
            {
                new LineAnnotation { OrderNumber = 1 },
                new LineAnnotation { OrderNumber = 3 },
                new LineAnnotation { OrderNumber = 5 },
                new LineAnnotation { OrderNumber = 4 },
                new LineAnnotation { OrderNumber = 2 }
            };

            var result = target.Validate(annotations);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }

        private LineOrderValidator target;
    }
}
