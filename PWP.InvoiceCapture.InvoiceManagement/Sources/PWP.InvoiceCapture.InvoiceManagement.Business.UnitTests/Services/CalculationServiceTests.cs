using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CalculationServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new CalculationService();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void CalculateHeight_WhenNewWidthIsLessOrEqualZero_ShouldArgumentException(int newWidth)
        {
            Assert.ThrowsException<ArgumentException>(() => target.CalculateHeight(newWidth, new SizeF(100, 100)));
        }

        [TestMethod]
        [DataRow(0, 100)]
        [DataRow(-1, 100)]
        [DataRow(100, 0)]
        [DataRow(100, -1)]
        public void CalculateHeight_WhenSizeIsLessOrEqualZero_ShouldArgumentException(float width, float height)
        {
            Assert.ThrowsException<ArgumentException>(() => target.CalculateHeight(1000, new SizeF(width, height)));
        }

        [TestMethod]
        [DataRow(100, 100, 100, 100)]
        [DataRow(50, 100, 100, 50)]
        [DataRow(200, 100, 100, 200)]
        public void CalculateHeight_WhenStreamContainsPdf_ShouldReturnNewHeight(int newWidth, float previousWidth, float previousHeight, int expectedHeight)
        {
            var height = target.CalculateHeight(newWidth, new SizeF(previousWidth, previousHeight));

            Assert.IsTrue(height == expectedHeight);
        }

        private CalculationService target;
    }
}
