using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Extensions
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void ToNullableDate_WhenValueIsNull_ShouldReturnNull()
        {
            var date = StringExtensions.ToNullableDate(null, invariantCultureInfo);

            Assert.IsNull(date);
        }

        [TestMethod]
        public void ToNullableDate_WhenValueIsNotNull_ShouldReturnDate()
        {
            var expectedDateTime = DateTime.UtcNow;
            var date = StringExtensions.ToNullableDate(expectedDateTime.ToString(), invariantCultureInfo);

            Assert.IsNotNull(date);

            var actualDate = date.Value;

            Assert.AreEqual(expectedDateTime.Year, actualDate.Year);
            Assert.AreEqual(expectedDateTime.Month, actualDate.Month);
            Assert.AreEqual(expectedDateTime.Day, actualDate.Day);
            Assert.AreEqual(0, actualDate.Hour);
            Assert.AreEqual(0, actualDate.Minute);
            Assert.AreEqual(0, actualDate.Second);
        }

        [TestMethod]
        public void ToNullableDateTime_WhenValueIsNull_ShouldReturnNull()
        {
            var dateTime = StringExtensions.ToNullableDateTime(null, invariantCultureInfo);

            Assert.IsNull(dateTime);
        }

        [TestMethod]
        public void ToNullableDateTime_WhenValueIsNotNull_ShouldReturnDateTime()
        {
            var expectedDateTime = DateTime.UtcNow;
            var dateTime = StringExtensions.ToNullableDateTime(expectedDateTime.ToString(), invariantCultureInfo);

            Assert.IsNotNull(dateTime);

            var actualDateTime = dateTime.Value;

            Assert.AreEqual(expectedDateTime.Year, actualDateTime.Year);
            Assert.AreEqual(expectedDateTime.Month, actualDateTime.Month);
            Assert.AreEqual(expectedDateTime.Day, actualDateTime.Day);
            Assert.AreEqual(expectedDateTime.Hour, actualDateTime.Hour);
            Assert.AreEqual(expectedDateTime.Minute, actualDateTime.Minute);
            Assert.AreEqual(expectedDateTime.Second, actualDateTime.Second);
        }

        [DataRow("25/11/2011", 25, 11, 2011)]
        [TestMethod]
        public void ToNullableDateTime_WhenValueIsFrenchCulture_ShouldReturnDateTime(string date, int day, int month, int year)
        {
            var actualDateTime = StringExtensions.ToNullableDateTime(date, frenchCultureInfo);

            Assert.IsNotNull(actualDateTime);

            var expectedDate = new DateTime(year, month, day);

            Assert.AreEqual(expectedDate.Year, actualDateTime.Value.Year);
            Assert.AreEqual(expectedDate.Month, actualDateTime.Value.Month);
            Assert.AreEqual(expectedDate.Day, actualDateTime.Value.Day);
        }

        [DataRow("11/25/2011", 25, 11, 2011)]
        [TestMethod]
        public void ToNullableDateTime_WhenValueIsUsCulture_ShouldReturnDateTime(string date, int day, int month, int year)
        {
            var actualDateTime = StringExtensions.ToNullableDateTime(date, uSCultureInfo);

            Assert.IsNotNull(actualDateTime);

            var expectedDate = new DateTime(year, month, day);

            Assert.AreEqual(expectedDate.Year, actualDateTime.Value.Year);
            Assert.AreEqual(expectedDate.Month, actualDateTime.Value.Month);
            Assert.AreEqual(expectedDate.Day, actualDateTime.Value.Day);
        }


        [TestMethod]
        public void ToNullableDecimal_WhenValueIsNull_ShouldReturnNull()
        {
            var decimalResult = StringExtensions.ToNullableDecimal(null, invariantCultureInfo);

            Assert.IsNull(decimalResult);
        }

        [TestMethod]
        [DataRow("12.0")]
        [DataRow("17.143")]
        [DataRow("178")]
        public void ToNullableDecimal_WhenValueIsNotNull_ShouldReturnDecimal(string value)
        {
            var expected = decimal.Parse(value);
            var actual = StringExtensions.ToNullableDecimal(value, invariantCultureInfo);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Value);
        }

        [TestMethod]
        [DataRow("12,5")]
        public void ToNullableDecimal_WhenValueIsFrenchCulture_ShouldReturnDecimal(string value)
        {
            var actual = StringExtensions.ToNullableDecimal(value, frenchCultureInfo);

            Assert.IsNotNull(actual);
            Assert.AreEqual(12.5m, actual.Value);
        }
       
        [TestMethod]
        [DataRow("12.5")]
        public void ToNullableDecimal_WhenValueIsInvariantCulture_ShouldReturnDecimal(string value)
        {
            var actual = StringExtensions.ToNullableDecimal(value, invariantCultureInfo);

            Assert.IsNotNull(actual);
            Assert.AreEqual(12.5m, actual.Value);
        }

        [TestMethod]
        public void FromHourOrDecimalToDecimal_WhenValueIsNull_ShouldReturn0()
        {
            var actual = StringExtensions.FromHourOrDecimalToDecimal(null, invariantCultureInfo);

            Assert.IsNotNull(actual);
            Assert.AreEqual(0m, actual);
        }

        [TestMethod]
        public void FromHourOrDecimalToDecimal_WhenValueIs0_ShouldReturn0()
        {
            var actual = StringExtensions.FromHourOrDecimalToDecimal("0", invariantCultureInfo);

            Assert.IsNotNull(actual);
            Assert.AreEqual(0m, actual);
        }

        [TestMethod]
        public void FromHourOrDecimalToDecimal_WhenValueIsDecimal_ShouldReturnDecimal()
        {
            var actual = StringExtensions.FromHourOrDecimalToDecimal("5.4", invariantCultureInfo);

            Assert.IsNotNull(actual);
            Assert.AreEqual(5.4m, actual);
        }

        [TestMethod]
        public void FromHourOrDecimalToDecimal_WhenValueHoursAndMinutes_ShouldReturnHoursAndMinutes()
        {
            var actual = StringExtensions.FromHourOrDecimalToDecimal("5:30", invariantCultureInfo);

            Assert.IsNotNull(actual);
            Assert.AreEqual(5.5m, actual);
        }

        [TestMethod]
        public void FromHourOrDecimalToDecimal_WhenValueHoursAndMinutesSeconds_ShouldReturnHoursAndMinutesAndSeconds()
        {
            var actual = StringExtensions.FromHourOrDecimalToDecimal("5:30:50", invariantCultureInfo);

            Assert.IsNotNull(actual);
            Assert.AreEqual(5.51m, actual);
        }

        private readonly CultureInfo invariantCultureInfo = CultureInfo.InvariantCulture;
        private readonly CultureInfo frenchCultureInfo = new CultureInfo("fr-FR");
        private readonly CultureInfo uSCultureInfo = new CultureInfo("en-US");
    }
}
