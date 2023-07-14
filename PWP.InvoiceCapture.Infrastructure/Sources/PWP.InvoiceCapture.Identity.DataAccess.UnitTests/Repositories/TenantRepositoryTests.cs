using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.UnitTests;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.TenantCapture.Identity.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class TenantRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new TenantRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new TenantRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenTenantIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 10)]
        [DataRow(100, 100)]
        public async Task CreateAsync_WhenTenantIsNotNull_ShouldSaveTenantAsync(int tenantId, int groupId)
        {
            var expectedTenant = CreateTenant(tenantId, TenantDatabaseStatus.NotCopied, groupId);
            expectedTenant.Id = 0;

            await target.CreateAsync(expectedTenant, cancellationToken);

            var actualTenant = context.Tenants.FirstOrDefault();

            Assert.IsNotNull(actualTenant);
            Assert.AreNotEqual(0, actualTenant.Id);
            Assert.AreNotEqual(default, actualTenant.CreatedDate);
            Assert.AreNotEqual(default, actualTenant.ModifiedDate);

            AssertTenantsAreEqual(expectedTenant, actualTenant);
        }

        [TestMethod]
        public async Task GetListAsync_WhenTenantsCollectionIsEmpty_ShouldReturnEmptyListAsync()
        {
            var actualTenants = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualTenants);
            Assert.AreEqual(0, actualTenants.Count);
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 10)]
        [DataRow(100, 100)]
        public async Task GetListAsync_WhenTenantsCollectionIsNotEmpty_ShouldReturnAllAsync(int expectedCount, int groupId)
        {
            var expectedTenants = Enumerable
                .Range(1, expectedCount)
                .Select(index => CreateTenant(index, TenantDatabaseStatus.NotCopied, groupId))
                .ToList();

            context.Tenants.AddRange(expectedTenants);
            context.SaveChanges();

            var actualTenants = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualTenants);
            Assert.AreEqual(expectedCount, actualTenants.Count);
        }

        [TestMethod]
        [DataRow(5, 5)]
        [DataRow(10, 10)]
        public async Task GetAsync_WhenTenantWithIdNotFound_ShouldReturnNullAsync(int tenantId, int groupId)
        {
            context.Tenants.Add(CreateTenant(tenantId - 1, TenantDatabaseStatus.NotCopied, groupId));
            context.SaveChanges();

            Assert.AreEqual(context.Tenants.Count(), 1);

            var actualTenant = await target.GetAsync(tenantId, cancellationToken);

            Assert.IsNull(actualTenant);
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 10)]
        public async Task GetAsync_WhenTenantWithIdExists_ShouldReturnValidObjectAsync(int tenantId, int groupId)
        {
            var expectedTenant = CreateTenant(tenantId, TenantDatabaseStatus.NotCopied, groupId);

            context.Tenants.Add(expectedTenant);
            context.SaveChanges();

            Assert.AreEqual(1, context.Tenants.Count());

            var actualTenant = await target.GetAsync(expectedTenant.Id, cancellationToken);

            Assert.IsNotNull(actualTenant);

            AssertTenantsAreEqual(expectedTenant, actualTenant);
        }

        [TestMethod]
        [DataRow(1, 1, 2)]
        [DataRow(10, 1, 1)]
        public async Task GetAsync_WhenTenantWithIdExistsAndContainUsers_ShouldReturnValidObjectAsync(int tenantId, int groupId, int expectedUsersCount)
        {
            var expectedTenant = CreateTenant(tenantId, TenantDatabaseStatus.NotCopied, groupId);

            context.Tenants.Add(expectedTenant);

            var expectedUsers = Enumerable
                .Range(1, expectedUsersCount)
                .Select(index => CreateUser(index, tenantId))
                .ToList();

            context.Users.AddRange(expectedUsers);

            context.SaveChanges();

            Assert.AreEqual(1, context.Tenants.Count());

            var actualTenant = await target.GetAsync(expectedTenant.Id, cancellationToken);

            Assert.IsNotNull(actualTenant);

            AssertTenantsAreEqual(expectedTenant, actualTenant);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetAsync_WhenTenantIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int tenantId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        public async Task UpdateTenantAsync_WhenTenantIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            Tenant tenant = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(tenant, cancellationToken));
        }

        [TestMethod]
        public async Task UpdateTenantAsync_WhenTenantsAreNull_ShouldThrowArgumentNullExceptionAsync()
        {
            List<Tenant> tenants = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(tenants, cancellationToken));
        }

        [TestMethod]
        [DataRow(int.MaxValue)]
        public async Task GetListExceptStatusAsync_WhenStatusIsWrong_ShouldThrowArgumentNullExceptionAsync(int statusValue)
        {
            var status = (TenantDatabaseStatus)statusValue;
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetListExceptStatusAsync(status, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, TenantDatabaseStatus.NotCopied, "newTenantName1", false, "newDbName1", 1)]
        [DataRow(123, TenantDatabaseStatus.Copied, "newTenantName2", true, "newDbName2", 123)]
        public async Task UpdateTenantAsync_WhenArgumentsAreValid_ShouldUpdateTenantAsync(
            int tenantId, 
            TenantDatabaseStatus newStatus, 
            string newName, 
            bool newIsActive, 
            string newDatabaseName,
            int groupId)
        {
            var tenant = CreateTenant(tenantId, TenantDatabaseStatus.NotCopied, groupId);

            context.Tenants.Add(tenant);
            context.SaveChanges();

            tenant.Status = newStatus;
            tenant.Name = newName;
            tenant.IsActive = newIsActive;
            tenant.DatabaseName = newDatabaseName;

            await target.UpdateAsync(tenant, cancellationToken);

            var actualTenant = context.Tenants.FirstOrDefault();

            Assert.AreEqual(actualTenant.Status, newStatus);
            Assert.AreEqual(actualTenant.Name, newName);
            Assert.AreEqual(actualTenant.IsActive, newIsActive);
            Assert.AreEqual(actualTenant.DatabaseName, newDatabaseName);
        }

        [TestMethod]
        [DataRow(1, TenantDatabaseStatus.NotCopied, "newTenantName1", false, "newDbName1", 1)]
        [DataRow(123, TenantDatabaseStatus.Copied, "newTenantName2", true, "newDbName2", 123)]
        public async Task UpdateTenantsAsync_WhenArgumentsAreValid_ShouldUpdateTenantAsync(
            int tenantId,
            TenantDatabaseStatus newStatus,
            string newName,
            bool newIsActive,
            string newDatabaseName,
            int groupId)
        {
            var tenant = CreateTenant(tenantId, TenantDatabaseStatus.NotCopied, groupId);

            context.Tenants.Add(tenant);
            context.SaveChanges();

            tenant.Status = newStatus;
            tenant.Name = newName;
            tenant.IsActive = newIsActive;
            tenant.DatabaseName = newDatabaseName;

            var tenants = new List<Tenant> { tenant };

            await target.UpdateAsync(tenants, cancellationToken);

            var actualTenant = context.Tenants.FirstOrDefault();

            Assert.AreEqual(actualTenant.Status, newStatus);
            Assert.AreEqual(actualTenant.Name, newName);
            Assert.AreEqual(actualTenant.IsActive, newIsActive);
            Assert.AreEqual(actualTenant.DatabaseName, newDatabaseName);
        }

        [TestMethod]
        [DataRow(TenantDatabaseStatus.Copied)]
        [DataRow(TenantDatabaseStatus.NotCopied)]
        public async Task GetListExceptStatusAsync_WhenTenantCollectionExists_ShouldReturnTenantsAsync(TenantDatabaseStatus exceptStatus)
        {
            var tenant1 = CreateTenant(1, TenantDatabaseStatus.NotCopied, 1);
            var tenant2 = CreateTenant(2, TenantDatabaseStatus.Copied, 2);

            context.Tenants.Add(tenant1);
            context.Tenants.Add(tenant2);
            context.SaveChanges();

            var actualTenants = await target.GetListExceptStatusAsync(exceptStatus, cancellationToken);

            var expectedTenant = context.Tenants.FirstOrDefault(tenant => tenant.Status != exceptStatus);

            Assert.AreEqual(actualTenants.Count(), 1);
            AssertTenantsAreEqual(actualTenants[0], expectedTenant);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("  ")]
        public async Task GetIdByUploadEmailAsync_WhenUploadEmailIsIncorrect_ShouldThrowArgumentNullExceptionAsync(string uploadEmail)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.GetIdByUploadEmailAsync(uploadEmail, cancellationToken));
        }

        [TestMethod]
        public async Task GetIdByUploadEmailAsync_ShouldReturnTenantAsync()
        {
            var emailToCheck = "someemailtocheck";
            var tenant1 = CreateTenant(1, TenantDatabaseStatus.Copied, 1, emailToCheck);
            var tenant2 = CreateTenant(2, TenantDatabaseStatus.Copied, 2);

            context.Tenants.Add(tenant1);
            context.Tenants.Add(tenant2);
            context.SaveChanges();

            var actualTenant = await target.GetIdByUploadEmailAsync(emailToCheck, cancellationToken);

            Assert.IsNotNull(actualTenant);
            AssertTenantsAreEqual(tenant1, actualTenant);
        }

        private Tenant CreateTenant(int id, TenantDatabaseStatus status, int groupId, string uploadedEmail = documentUploadEmail)
        {
            return new Tenant
            {
                Id = id,
                Name = $"Name{id}",
                Status = status,
                IsActive = true,
                DatabaseName = $"DatabaseName{id}",
                DocumentUploadEmail = uploadedEmail,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                GroupId = groupId
            };
        }

        private User CreateUser(int id, int groupId)
        {
            return new User
            {
                Id = id,
                GroupId = groupId,
                IsActive = true,
                PasswordHash = "PasswordHash",
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }

        private void AssertTenantsAreEqual(Tenant expected, Tenant actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.IsActive, actual.IsActive);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.DatabaseName, actual.DatabaseName);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.GroupId, actual.GroupId);
            Assert.AreEqual(expected.DocumentUploadEmail, actual.DocumentUploadEmail);

            // Ensure all properties are tested
            Assert.AreEqual(9, actual.GetType().GetProperties().Length);
        }

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private ITenantRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const string documentUploadEmail = "sometest@email.com";
    }
}
