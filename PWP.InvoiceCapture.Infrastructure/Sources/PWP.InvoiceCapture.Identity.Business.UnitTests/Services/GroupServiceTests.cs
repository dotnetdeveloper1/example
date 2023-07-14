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
    public class GroupServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            groupRepositoryMock = mockRepository.Create<IGroupRepository>();
            userServiceMock = mockRepository.Create<IUserService>();
            target = new GroupService(groupRepositoryMock.Object, userServiceMock.Object);
        }

        [TestMethod]
        public void Instance_WhenGroupRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new GroupService(null, userServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenUserServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new GroupService(groupRepositoryMock.Object, null));
        }

        [TestMethod]
        public async Task GetListAsync_WhenGroupCollectionExists_ShouldReturnGroups()
        {
            var expectedGroups = new List<Group>() { CreateGroup(1, "group 1") };

            groupRepositoryMock
                .Setup(groupRepository => groupRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedGroups);

            var actualGroups = await target.GetListAsync(cancellationToken);

            Assert.AreEqual(actualGroups.Count(), 1);
            AssertGroupsAreEqual(actualGroups[0], expectedGroups[0]);
        }

        [TestMethod]
        public async Task GetListAsync_WhenGroupRepositoryThrowError_ShouldThrowTimeoutException()
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.GetListAsync(cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetListAsync(cancellationToken));
        }

        [TestMethod]
        [DataRow("groupName 1", 1)]
        [DataRow("groupName 123", 2)]
        public async Task CreateAsync_WhenUserWasSaved_ReturnSuccessOperationResult(string groupName, int groupId)
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.CreateAsync(It.IsAny<Group>(), cancellationToken))
                .ReturnsAsync(groupId);
            userServiceMock
                .Setup(userService => userService.CreateAsync(groupId, cancellationToken))
                .ReturnsAsync(new Core.Models.OperationResult<UserCreationResponse>() { Status = OperationResultStatus.Success });


            var groupCreationParameters = CreateGroupCreationParameters(groupName, null);

            var actualResult = await target.CreateAsync(groupCreationParameters, cancellationToken);

            Assert.IsTrue(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Success);
            Assert.AreEqual(actualResult.Message, $"Group with name '{groupCreationParameters.Name}' created.");
            Assert.IsNotNull(actualResult.Data);
            Assert.AreEqual(actualResult.Data.Id, groupId);
            Assert.AreEqual(actualResult.Data.Name, groupName);
        }

        [TestMethod]
        [DataRow("groupName 1", 1)]
        public async Task CreateAsync_WhenParentGroupDoesntExist_ReturnErrorOperationResult(string groupName, int parentGroupId)
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(parentGroupId, cancellationToken))
                .ReturnsAsync(false);

            var groupCreationParameters = CreateGroupCreationParameters(groupName, parentGroupId);

            var actualResult = await target.CreateAsync(groupCreationParameters, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Failed);
            Assert.AreEqual(actualResult.Message, "Parent group not found.");
        }

        private Group CreateGroup(int id, string name)
        {
            return new Group()
            {
                Id = id,
                Name = name,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
            };
        }

        private void AssertGroupsAreEqual(Group actual, Group expected)
        {
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.ParentGroupId, actual.ParentGroupId);
            Assert.AreEqual(expected.Id, actual.Id);
        }

        private GroupCreationParameters CreateGroupCreationParameters(string name, int? parentGroupId = null)
        {
            return new GroupCreationParameters()
            {
                Name = name,
                ParentGroupId = parentGroupId
            };
        }

        private MockRepository mockRepository;
        private Mock<IGroupRepository> groupRepositoryMock;
        private Mock<IUserService> userServiceMock;
        private GroupService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
