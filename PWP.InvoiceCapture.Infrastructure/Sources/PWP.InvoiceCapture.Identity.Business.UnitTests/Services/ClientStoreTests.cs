using IdentityServer4.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ClientStoreTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            applicationClientServiceMock = mockRepository.Create<IApplicationClientService>();
            target = new ClientStore(Options.Create(options), applicationClientServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenApplicationClientServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ClientStore(Options.Create(options), null));
        }

        [TestMethod]
        public void Instance_WhenOptionsAccessorAreNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ClientStore(null, applicationClientServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenOptionsAreNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<AuthenticationServerOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() => new ClientStore(optionsAccessor, applicationClientServiceMock.Object));
        }

        [TestMethod]
        [DataRow(-123)]
        [DataRow(0)]
        public void Instance_WhenAccessTokenLifetimeInSecondsIsZeroOrLess_ShouldThrowArgumentException(int accessTokenLifetimeInSeconds)
        {
            options.AccessTokenLifetimeInSeconds = accessTokenLifetimeInSeconds;
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentException>(() => new ClientStore(optionsAccessor, applicationClientServiceMock.Object));
        }

        [TestMethod]
        [DataRow(-123)]
        [DataRow(0)]
        public void Instance_WhenRefreshTokenLifetimeInSecondsIsZeroOrLess_ShouldThrowArgumentException(int refreshTokenLifetimeInSeconds)
        {
            options.RefreshTokenLifetimeInSeconds = refreshTokenLifetimeInSeconds;
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentException>(() => new ClientStore(optionsAccessor, applicationClientServiceMock.Object));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("    ")]
        public void Instance_WhenSigningKeyIsNullOrWhitespace_ShouldThrowArgumentException(string signingKey)
        {
            options.SigningKey = signingKey;
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new ClientStore(optionsAccessor, applicationClientServiceMock.Object));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("    ")]
        public async Task FindClientByIdAsync_WhenClientIdIsNullOrWhiteSpace_ShouldReturnNull(string clientId)
        {
            var client = await target.FindClientByIdAsync(clientId);

            Assert.IsNull(client);
        }

        [TestMethod]
        [DataRow("clientId_1")]
        [DataRow("clientId_2")]
        public async Task FindClientByIdAsync_WhenClientIdNotExists_ShouldReturnNull(string clientId)
        {
            applicationClientServiceMock
                .Setup(applicationClientService => applicationClientService.GetAsync(clientId, CancellationToken.None))
                .ReturnsAsync((ApplicationClient)null);

            var client = await target.FindClientByIdAsync(clientId);

            Assert.IsNull(client);
        }

        [TestMethod]
        [DataRow("defaultClient")]
        [DataRow("webApplication")]
        public async Task FindClientByIdAsync_WhenClientIdExists_ShouldReturnClient(string clientId)
        {
            applicationClientServiceMock
                .Setup(applicationClientService => applicationClientService.GetAsync(clientId, CancellationToken.None))
                .ReturnsAsync(CreateApplicationClient(clientId));

            var client = await target.FindClientByIdAsync(clientId);

            Assert.IsNotNull(client);
            Assert.AreEqual(clientId, client.ClientId);
        }

        private ApplicationClient CreateApplicationClient(string clientId)
        {
            return new ApplicationClient()
            {
                ClientId = clientId
            };
        }

        private readonly AuthenticationServerOptions options = new AuthenticationServerOptions()
        {
            AccessTokenLifetimeInSeconds = 60,
            RefreshTokenLifetimeInSeconds = 120,
            SigningKey = "SigningKey"
        };

        private MockRepository mockRepository;
        private ClientStore target;
        private Mock<IApplicationClientService> applicationClientServiceMock;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
