using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.UnitTests.Extensions
{
    [TestClass]
    public class ExtensionTests
    {
        [DataRow("13/02/2020", DataType.Date)]
        [DataRow("02/13/2020", DataType.Date)]
        [DataRow("2/13/2020", DataType.Date)]
        [DataRow("13/2/2020", DataType.Date)]
        [DataRow("13-02-2020", DataType.Date)]
        [DataRow("02-13-2020", DataType.Date)]
        [DataRow("2-13-2020", DataType.Date)]
        [DataRow("13-2-2020", DataType.Date)]
        [DataRow("Feb 25, 2017", DataType.Date)]
        [DataRow("September 24th 2017", DataType.Date)]
        [DataRow("28-JAN-15", DataType.Date)]
        [DataRow("September 18, 2017", DataType.Date)]
        [DataRow("30AUG2019", DataType.Date)]
        [DataRow("31 Jan 2019", DataType.Date)]
        [DataRow("30AUG19", DataType.Date)]
        [DataRow("31 Jan 19", DataType.Date)]
        [DataRow("Feb 25, 19", DataType.Date)]
        [DataRow("28.12.2020", DataType.Date)]
        [DataRow("28/12/2020", DataType.Date)]
        [DataRow("14-08-2019", DataType.Date)]
        [DataRow("28.12.19", DataType.Date)]
        [DataRow("28/12/17", DataType.Date)]
        [DataRow("14-08-19", DataType.Date)]
        [DataRow("12.30.2020", DataType.Date)]
        [DataRow("12/28/2020", DataType.Date)]
        [DataRow("08-14-2019", DataType.Date)]
        [DataRow("12.30.20", DataType.Date)]
        [DataRow("12/28/16", DataType.Date)]
        [DataRow("08-14-14", DataType.Date)]
        [DataRow("1030,20", DataType.Number)]
        [DataRow("1,030.20", DataType.Number)]
        [TestMethod]
        public void String_WhenConfromsToDataType_ShouldBeOfDataType(string value, DataType type)
        {
            Assert.AreEqual(type, value.GetDataType());
        }

        [DataRow("0", "0", true)]
        // Dash (-) will be converted to 0
        [DataRow("-", "0", true)]
        [DataRow(" - ", "0", true)]
        // Negative sign should parse out correctly
        [DataRow("-1000", "-1000", false)]
        [DataRow("-1000$", "-1000", false)]
        [DataRow("- 1000$", "-1000", false)]
        [DataRow("-1,030.20 AUD", "-1030.20", false)]
        [DataRow ("1030,20", "1030.20", false)]
        [DataRow("1 030,20 $", "1030.20", false)]
        [DataRow("$ 1 030,20", "1030.20", false)]
        [DataRow("1.030,20 USD", "1030.20", false)]
        [DataRow("1,030.20 EUR", "1030.20", false)]
        [DataRow("$1000", "1000", false)]
        [DataRow("USD1000", "1000", false)]
        // Paranthesis for negative values, and combinations with previous rules
        [DataRow("(1000)", "-1000", false)]
        [DataRow("(1,030.20 EUR)", "-1030.20", false)]
        [DataRow("(1 030,20 $)", "-1030.20", false)]
        [DataRow("($ 1 030,20)", "-1030.20", false)]
        [DataRow("(1,030.20 AUD)", "-1030.20", false)]
        [TestMethod]
        public void String_WhenConfromsToPrice_ShouldBeParsedToCorrectFormat(string value, string expectedValue, bool replaceDash )
        {
            Assert.AreEqual(value.GetAnnotatedValue(replaceDash), expectedValue, replaceDash);
        }
    }
}

