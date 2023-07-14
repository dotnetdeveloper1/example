using PWP.InvoiceCapture.Core.ServiceBus.Models;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal class FakeMessageWithInheritance : ServiceBusMessageBase
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }
}
