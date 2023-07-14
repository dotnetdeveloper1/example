using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.UnitTests.Utilities
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class GuardTests
    {
        [TestMethod]
        public void IsNotNull_WhenArgumentIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Guard.IsNotNull(null, "parameterName"));
        }

        [TestMethod]
        public void IsNotNull_WhenArgumentIsNotNull_ShouldNotThrowException()
        {
            var data = new object();

            Guard.IsNotNull(data, nameof(data));
        }

        [TestMethod]
        public void IsNotNullOrWhiteSpace_WhenArgumentIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Guard.IsNotNullOrWhiteSpace(null, "parameterName"));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public void IsNotNullOrWhiteSpace_WhenArgumentIsEmpty_ShouldThrowArgumentNullException(string value)
        {
            Assert.ThrowsException<ArgumentNullException>(() => Guard.IsNotNullOrWhiteSpace(value, nameof(value)));
        }

        [TestMethod]
        public void IsNotNullOrWhiteSpace_WhenArgumentIsNotNull_ShouldNotThrowException()
        {
            Guard.IsNotNullOrWhiteSpace("stringValue", "parameterName");
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(int.MaxValue)]
        public void IsNotZeroOrNegative_WhenArgumentIsNotZeroOrNegative_ShouldNotThrowException(int value)
        {
            Guard.IsNotZeroOrNegative(value, nameof(value));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public void IsNotZeroOrNegative_WhenArgumentIsZeroOrNegative_ShouldThrowArgumentException(int value)
        {
            Assert.ThrowsException<ArgumentException>(() => Guard.IsNotZeroOrNegative(value, nameof(value)));
        }

        [TestMethod]
        public void IsNotNullOrEmpty_WhenArgumentIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Guard.IsNotNullOrEmpty<byte[]>(null, "parameterName"));
        }

        [TestMethod]
        public void IsNotNullOrEmpty_WhenArgumentIsEmpty_ShouldThrowArgumentNullException()
        {
            var collection = new byte[] { };

            Assert.ThrowsException<ArgumentNullException>(() => Guard.IsNotNullOrEmpty(collection, nameof(collection)));
        }

        [TestMethod]
        [DataRow(new byte[] { 0x01 })]
        [DataRow(new byte[] { 0x01, 0x02 })]
        public void IsNotNullOrEmpty_WhenArgumentIsNotNullAndNotEmpty_ShouldNotThrowArgumentNullException(byte[] collection)
        {
            Guard.IsNotNullOrEmpty(collection, nameof(collection));
        }

        [DataRow(154)]
        [TestMethod]
        public void IsEnumDefined_WhenValueIsIncorrect_ShouldThrowException(int value)
        {
            Assert.ThrowsException<ArgumentException>(() => Guard.IsEnumDefined((OperationResultStatus)value, nameof(value)));
        }

        [DataRow(1)]
        [TestMethod]
        public void IsEnumDefined_WhenValueIsCorrect_ShouldNotThrowException(int value)
        {
            Guard.IsEnumDefined((OperationResultStatus)value, nameof(value));
        }
    }
}
