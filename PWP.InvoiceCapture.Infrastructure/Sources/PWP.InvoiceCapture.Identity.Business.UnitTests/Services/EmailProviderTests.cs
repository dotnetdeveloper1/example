using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EmailProviderTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            nameGeneratorMock = mockRepository.Create<INameGenerator>();
            target = new EmailProvider(Options.Create(options), nameGeneratorMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenOptionsAccessorAreNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new EmailProvider(null, nameGeneratorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenNameGeneratorIsNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<EmailAddressGenerationOptions>(null);
            Assert.ThrowsException<ArgumentNullException>(() => new EmailProvider(optionsAccessor, null));
        }

        [TestMethod]
        public void Instance_WhenOptionsAreNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<EmailAddressGenerationOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() => new EmailProvider(optionsAccessor, nameGeneratorMock.Object));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("    ")]
        public void Instance_WhenPostfixIsNullOrWhitespace_ShouldThrowArgumentException(string postfix)
        {
            options.Postfix = postfix;
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new EmailProvider(optionsAccessor, nameGeneratorMock.Object));
        }

        [TestMethod]
        public void Generate_ShouldGenerateUniqEmail()
        {
            nameGeneratorMock
                .Setup(nameGenerator => nameGenerator.GenerateName())
                .Returns(uniqName);

            var email =  target.Generate();

            Assert.IsFalse(string.IsNullOrWhiteSpace(email));
            Assert.AreEqual(email.Split("@")[0], uniqName);
            Assert.AreEqual(email.Split("@")[1], "emailinvoice.workplacecloud.com");
        }

        private ApplicationClient CreateApplicationClient(string clientId)
        {
            return new ApplicationClient()
            {
                ClientId = clientId
            };
        }

        private readonly EmailAddressGenerationOptions options = new EmailAddressGenerationOptions()
        {
            Postfix = "@emailinvoice.workplacecloud.com"
        };

        private MockRepository mockRepository;
        private Mock<INameGenerator> nameGeneratorMock;
        private EmailProvider target;
        private const string uniqName = "1a2b3c4g";
    }
}
