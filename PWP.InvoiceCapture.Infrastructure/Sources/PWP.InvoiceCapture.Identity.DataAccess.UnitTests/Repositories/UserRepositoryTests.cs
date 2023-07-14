using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace PWP.tenantCapture.tenantManagement.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class UserRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            cancellationToken = CancellationToken.None;
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new UserRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new UserRepository(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetListAsync_WhenTenantIdIsZeroOrLess_ShouldThrowArgumentExceptionAsync(int tenantId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetListAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetAsync_WhenUserIdIsZeroOrLess_ShouldThrowArgumentExceptionAsync(int userId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(userId, cancellationToken));
        }

        [TestMethod]
        [DataRow(100)]
        public async Task GetListAsync_WhenUsersCollectionIsNotEmpty_ShouldReturnAllAsync(int count)
        {
            var users = Enumerable
                .Range(groupId, count)
                .Select(index => CreateUser(index))
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            var actualUsers = await target.GetListAsync(groupId, cancellationToken);

            Assert.IsNotNull(actualUsers);
            Assert.AreEqual(count, actualUsers.Count);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetListAsync_WhenTenantIdNotExists_ShouldReturnEmptyListAsync(int count)
        {
            var users = Enumerable
                .Range(groupId, count)
                .Select(index => CreateUser(index))
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            var actualUsers = await target.GetListAsync(notExistingTenantId, cancellationToken);

            Assert.IsNotNull(actualUsers);
            Assert.AreEqual(0, actualUsers.Count);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetAsync_WhenUserIdExists_ShouldReturnUserAsync(int count)
        {
            var users = Enumerable
                .Range(groupId, count)
                .Select(index => CreateUser(index))
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            for (var userId = 1; userId <= count; userId++)
            {
                var actualtenantPage = await target.GetAsync(userId, cancellationToken);

                AssertUsersAreEqual(users[userId - 1], actualtenantPage);
            }
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetAsync_WhenUserNotExists_ShouldReturnNullAsync(int count)
        {
            var users = Enumerable
                .Range(groupId, count)
                .Select(index => CreateUser(index))
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            var actualUser = await target.GetAsync(notExistingTenantId, cancellationToken);

            Assert.IsNull(actualUser);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task IsUsernameExistsAsync_WhenUserExists_ShouldReturnTrueAsync(int count)
        {
            var users = Enumerable
                .Range(groupId, count)
                .Select(index => CreateUser(index))
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            var existingUsername = users.Select(user => user.Username).Last();

            var isUsernameExists = await target.IsUsernameExistsAsync(existingUsername, cancellationToken);

            Assert.IsTrue(isUsernameExists);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task IsUsernameExistsAsync_WhenUsernameNotExists_ShouldReturnFalseAsync(int count)
        {
            var users = Enumerable
                .Range(groupId, count)
                .Select(index => CreateUser(index))
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            var notExistingUsername = Guid.NewGuid().ToString();

            var isUsernameExists = await target.IsUsernameExistsAsync(notExistingUsername, cancellationToken);

            Assert.IsFalse(isUsernameExists);
        }

        [TestMethod]
        public async Task CreateAsync_WhenUserIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            User user = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(user, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenUserListIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            List<User> users = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(users, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenUserListIsEmpty_ShouldThrowArgumentNullExceptionAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(new List<User>(), cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_ShouldSaveUsersAsync()
        {
            var users = new List<User>();
            users.Add(CreateUser(1));
            users.Add(CreateUser(2));

            await target.CreateAsync(users, cancellationToken);

            var result = context.Users.ToList();

            Assert.AreEqual(result.Count, 2);

            AssertUsersAreEqual(users[0], result[0]);
            AssertUsersAreEqual(users[1], result[1]);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldSaveUserAsync()
        {
            var user = CreateUser(1);

            await target.CreateAsync(user, cancellationToken);

            var result = context.Users.ToList();

            Assert.AreEqual(result.Count, 1);

            AssertUsersAreEqual(user, result[0]);
        }

        private User CreateUser(int id)
        {
            return new User
            {
                Id = id,
                GroupId = groupId,
                IsActive = true,
                PasswordHash = "PasswordHash",
                Username = $"Username_{id}",
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }

        private void AssertUsersAreEqual(User expected, User actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.IsActive, actual.IsActive);
            Assert.AreEqual(expected.GroupId, actual.GroupId);
            Assert.AreEqual(expected.PasswordHash, actual.PasswordHash);
            Assert.AreEqual(expected.Username, actual.Username);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);

            // Ensure all properties are tested
            Assert.AreEqual(7, actual.GetType().GetProperties().Length);
        }

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private IUserRepository target;
        private CancellationToken cancellationToken;

        private const int groupId = 1;
        private const int notExistingTenantId = 123123;
    }
}
