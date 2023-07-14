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
    public class UserServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            userRepositoryMock = mockRepository.Create<IUserRepository>();
            groupRepositoryMock = mockRepository.Create<IGroupRepository>();
            passwordHashServiceMock = mockRepository.Create<IPasswordHashService>();
            passwordGeneratorMock = mockRepository.Create<IPasswordGenerator>();
            nameGeneratorMock = mockRepository.Create<INameGenerator>();

            target = new UserService(userRepositoryMock.Object, groupRepositoryMock.Object, passwordHashServiceMock.Object, passwordGeneratorMock.Object, nameGeneratorMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenUserRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserService(null, groupRepositoryMock.Object, passwordHashServiceMock.Object, passwordGeneratorMock.Object, nameGeneratorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenGroupRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserService(userRepositoryMock.Object, null, passwordHashServiceMock.Object, passwordGeneratorMock.Object, nameGeneratorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenPasswordHashServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserService(userRepositoryMock.Object, groupRepositoryMock.Object, null, passwordGeneratorMock.Object, nameGeneratorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenPasswordGeneratorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserService(userRepositoryMock.Object, groupRepositoryMock.Object, passwordHashServiceMock.Object, null, nameGeneratorMock.Object));
        }

        [TestMethod]
        public void Instance_WhenNameGeneratorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserService(userRepositoryMock.Object, groupRepositoryMock.Object, passwordHashServiceMock.Object, passwordGeneratorMock.Object, null));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(123)]
        public async Task GetListAsync_WhenUserCollectionExists_ShouldReturnUsers(int tenantId)
        {
            var expectedUsers = new List<User>() { CreateUser("user 1", tenantId) };

            userRepositoryMock
                .Setup(userRepository => userRepository.GetListAsync(tenantId, cancellationToken))
                .ReturnsAsync(expectedUsers);

            var actualUsers = await target.GetListAsync(tenantId, cancellationToken);

            Assert.AreEqual(actualUsers.Count(), 1);
            AssertUsersAreEqual(actualUsers[0], expectedUsers[0]);
        }

        [TestMethod]
        [DataRow("  ", "username 1", 1)]
        [DataRow(null, "username 2", 10)]
        public async Task CreateAsync_WhenPasswordIsNullOrWhitespace_ShouldReturnFailedOperationResult(string password, string username, int groupId)
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(true);

            nameGeneratorMock
                .Setup(nameGenerator => nameGenerator.GenerateName())
                .Returns(username);

            userRepositoryMock
                .Setup(userRepository => userRepository.IsUsernameExistsAsync(username, cancellationToken))
                .ReturnsAsync(false);

            passwordGeneratorMock
                .Setup(passwordGenerator => passwordGenerator.GeneratePassword())
                .Returns(password);

            var actualResult = await target.CreateAsync(groupId, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, actualResult.Status);
            Assert.AreEqual("Can't generate user.", actualResult.Message);
        }

        [TestMethod]
        [DataRow("  ", 1)]
        [DataRow(null, 10)]
        public async Task CreateAsync_WhenUsernameIsNullOrWhitespace_ShouldReturnFailedOperationResult(string username, int groupId)
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(true);

            nameGeneratorMock
                .Setup(nameGenerator => nameGenerator.GenerateName())
                .Returns(username);

            var actualResult = await target.CreateAsync(groupId, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, actualResult.Status);
            Assert.AreEqual("Can't generate user.", actualResult.Message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(121)]
        public async Task CreateAsync_WhenGroupNotExists_ReturnFailedOperationResult(int groupId)
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(false);

            var actualResult = await target.CreateAsync(groupId, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Failed);
            Assert.AreEqual(actualResult.Message, $"Group with id {groupId} doesn't exist.");
        }

        [TestMethod]
        [DataRow("userName 1", 2)]
        [DataRow("userName 123", 3)]
        public async Task CreateAsync_WhenUserNameAlreadyExists_ReturnFailedOperationResult(string userName, int groupId)
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(true);

            nameGeneratorMock
                .Setup(nameGenerator => nameGenerator.GenerateName())
                .Returns(userName);

            userRepositoryMock
                .Setup(userRepository => userRepository.IsUsernameExistsAsync(userName, cancellationToken))
                .ReturnsAsync(true);

            var actualResult = await target.CreateAsync(groupId, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Failed);
            Assert.AreEqual(actualResult.Message, $"User with name '{userName}' already exists.");
        }

        [TestMethod]
        [DataRow("userName 1", 1, 1)]
        [DataRow("userName 123", 32, 2)]
        public async Task CreateAsync_WhenUserWasSaved_ReturnSuccessOperationResult(string userName, int groupId, int userId)
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(true);

            nameGeneratorMock
                .Setup(nameGenerator => nameGenerator.GenerateName())
                .Returns(userName);

            userRepositoryMock
                .Setup(userRepository => userRepository.IsUsernameExistsAsync(userName, cancellationToken))
                .ReturnsAsync(false);

            userRepositoryMock
                .Setup(userRepository => userRepository.CreateAsync(It.IsAny<User>(), cancellationToken))
                .ReturnsAsync(userId);

            passwordGeneratorMock
                .Setup(passwordGenerator => passwordGenerator.GeneratePassword())
                .Returns("password");

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns("passwordHash");

            var actualResult = await target.CreateAsync(groupId, cancellationToken);

            Assert.IsTrue(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Success);
            Assert.AreEqual(actualResult.Message, $"User with name '{userName}' created.");
            Assert.IsNotNull(actualResult.Data);
            Assert.AreEqual(actualResult.Data.Id, userId);
            Assert.AreEqual(actualResult.Data.Username, userName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(actualResult.Data.Password));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(123)]
        public async Task GetListAsync_WhenUserRepositoryThrowError_ShouldThrowTimeoutException(int tenantId)
        {
            userRepositoryMock
                .Setup(userRepository => userRepository.GetListAsync(tenantId, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetListAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetListAsync_WhenTenantIdIsZeroOrNegative_ShouldThrowArgumentException(int tenantId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetListAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetAsync_WhenUserIdIsZeroOrNegative_ShouldThrowArgumentException(int userId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetAsync(userId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenUserNotFound_ShouldReturnNull(int userId)
        {
            userRepositoryMock
                .Setup(userRepository => userRepository.GetAsync(userId, cancellationToken))
                .ReturnsAsync((User)null);

            var actualUser = await target.GetAsync(userId, cancellationToken);

            Assert.IsNull(actualUser);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenUserFound_ShouldReturnUser(int userId)
        {
            var expectedUser = CreateUser("user name", 1);

            userRepositoryMock
                .Setup(userRepository => userRepository.GetAsync(userId, cancellationToken))
                .ReturnsAsync(expectedUser);

            var actualUser = await target.GetAsync(userId, cancellationToken);
            
            AssertUsersAreEqual(actualUser, expectedUser);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenUserRepositoryThrowError_ShouldThrowTimeoutException(int userId)
        {
            userRepositoryMock
                .Setup(userRepository => userRepository.GetAsync(userId, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetAsync(userId, cancellationToken));
        }

        [TestMethod]
        [DataRow("  ")]
        [DataRow("")]
        [DataRow(null)]
        public void GetAsync_WhenUsernameIsNullOrWhitespace_ShouldThrowArgumentException(string userName)
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.GetAsync(userName, cancellationToken));
        }

        [TestMethod]
        [DataRow("username 1")]
        [DataRow("username 2")]
        public async Task GetAsync_WhenUserRepositoryThrowError_ShouldThrowTimeoutException(string userName)
        {
            userRepositoryMock
                .Setup(userRepository => userRepository.GetAsync(userName, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetAsync(userName, cancellationToken));
        }

        [TestMethod]
        [DataRow("username 1")]
        [DataRow("username 2")]
        public async Task GetAsync_WhenUsernameExistsInRepository_ShouldReturnUser(string userName)
        {
            var expectedUser = CreateUser(userName, 1);

            userRepositoryMock
                .Setup(userRepository => userRepository.GetAsync(userName, cancellationToken))
                .ReturnsAsync(expectedUser);

            var actualUser = await target.GetAsync(userName, cancellationToken);

            AssertUsersAreEqual(actualUser, expectedUser);
        }

        [TestMethod]
        [DataRow("username 1")]
        [DataRow("username 2")]
        public async Task GetAsync_WhenUsernameNotExistsInRepository_ShouldReturnNull(string userName)
        {
            userRepositoryMock
                .Setup(userRepository => userRepository.GetAsync(userName, cancellationToken))
                .ReturnsAsync((User)null);

            var actualUser = await target.GetAsync(userName, cancellationToken);

            Assert.IsNull(actualUser);
        }

        private void AssertUsersAreEqual(User actual, User expected)
        {
            Assert.AreEqual(expected.PasswordHash, actual.PasswordHash);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.IsActive, actual.IsActive);
            Assert.AreEqual(expected.Username, actual.Username);
            Assert.AreEqual(expected.GroupId, actual.GroupId);
            Assert.AreEqual(expected.Id, actual.Id);
        }

        private User CreateUser(string userName, int groupId)
        {
            return new User()
            {
                Id = groupId,
                PasswordHash = "PasswordHash",
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                GroupId = groupId,
                Username = userName
            };
        }

        private MockRepository mockRepository;
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IGroupRepository> groupRepositoryMock;
        private Mock<IPasswordHashService> passwordHashServiceMock;
        private Mock<IPasswordGenerator> passwordGeneratorMock;
        private Mock<INameGenerator> nameGeneratorMock;
        private UserService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
