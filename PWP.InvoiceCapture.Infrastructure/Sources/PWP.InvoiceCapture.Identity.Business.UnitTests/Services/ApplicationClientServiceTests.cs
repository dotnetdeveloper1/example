using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ApplicationClientServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            aplicationClientRepositoryMock = mockRepository.Create<IApplicationClientRepository>();
            target = new ApplicationClientService(aplicationClientRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenAplicationClientRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApplicationClientService(null));
        }

        [TestMethod]
        [DataRow(10)]
        [DataRow(15)]
        public async Task GetAsync_WhenApplicationClientExists_ShouldReturnUsers(int id)
        {
            var expectedApplicationClient = CreateApplicationClient(id);
            var clientId = expectedApplicationClient.ClientId;

            aplicationClientRepositoryMock
                .Setup(aplicationClientRepository => aplicationClientRepository.GetAsync(clientId, cancellationToken))
                .ReturnsAsync(expectedApplicationClient);

            var actualApplicationClient = await target.GetAsync(clientId, cancellationToken);

            AssertUsersAreEqual(actualApplicationClient, expectedApplicationClient);
        }

        private ApplicationClient CreateApplicationClient(int id)
        {
            return new ApplicationClient
            {
                Id = id,
                ClientId = $"ClientId_{id}",
                IsActive = true,
                SecretHash = "SecretHash",
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }

        private void AssertUsersAreEqual(ApplicationClient expected, ApplicationClient actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.IsActive, actual.IsActive);
            Assert.AreEqual(expected.ClientId, actual.ClientId);
            Assert.AreEqual(expected.SecretHash, actual.SecretHash);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);

            // Ensure all properties are tested
            Assert.AreEqual(6, actual.GetType().GetProperties().Length);
        }

        private MockRepository mockRepository;
        private Mock<IApplicationClientRepository> aplicationClientRepositoryMock;
        private ApplicationClientService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
