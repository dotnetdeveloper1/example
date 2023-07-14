using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal class FakeTestObject
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public FakeInnerTestObject InnerObject { get; set; }
    }
}
