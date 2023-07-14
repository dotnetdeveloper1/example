using IdentityModel;
using IdentityServer4.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.UnitTests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.tenantCapture.tenantManagement.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ApplicationClientRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            cancellationToken = CancellationToken.None;
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new ApplicationClientRepository(contextFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public void Instance_WhenDbContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApplicationClientRepository(null));
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetAsync_WhenApplicationClientExists_ShouldReturnApplicationClientAsync(int count)
        {
            var expectedClients = Enumerable
                .Range(1, count)
                .Select(index => CreateApplicationClient(index))
                .ToList();

            context.ApplicationClients.AddRange(expectedClients);
            context.SaveChanges();

            var expectedClient = expectedClients.Last();

            var actualClient = await target.GetAsync(expectedClient.ClientId, cancellationToken);

            AssertUsersAreEqual(expectedClient, actualClient);
        }

        [TestMethod]
        [DataRow("    ")]
        [DataRow("")]
        [DataRow(null)]
        public void GetAsync_WhenClientIdIsNullOrWhitespace_ShouldThrowArgumentNullException(string wrongClientId)
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.GetAsync(wrongClientId, cancellationToken));
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

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private IApplicationClientRepository target;
        private CancellationToken cancellationToken;
    }
}
