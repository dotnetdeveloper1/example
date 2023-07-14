using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal class FakeMessage
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }
}
