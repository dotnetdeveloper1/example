using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Services;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PasswordGeneratorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new PasswordGenerator();
        }

        [TestMethod]
        public void GetPassword_ShouldReturnPassword()
        {
            var password = target.GeneratePassword();

            Assert.IsFalse(string.IsNullOrEmpty(password));
            Assert.IsTrue(password.Length > 6);
        }

        private PasswordGenerator target;
    }
}
