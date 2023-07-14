using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Services;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UniqueNameGeneratorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new UniqueNameGenerator();
        }

        [TestMethod]
        public void GenerateName_ShouldReturnName()
        {
            var name = target.GenerateName();

            Assert.IsFalse(string.IsNullOrWhiteSpace(name));
            Assert.IsFalse(name.Contains("-"));
        }

        private UniqueNameGenerator target;
    }
}
