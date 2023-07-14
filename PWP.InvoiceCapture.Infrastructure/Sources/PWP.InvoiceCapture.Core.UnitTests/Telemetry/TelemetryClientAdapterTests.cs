using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.Telemetry;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.UnitTests.Telemetry
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class TelemetryClientAdapterTests
    {
        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TelemetryClientAdapter(null));
        }
    }
}
