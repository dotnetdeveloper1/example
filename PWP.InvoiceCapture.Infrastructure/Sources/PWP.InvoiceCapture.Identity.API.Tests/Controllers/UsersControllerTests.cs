using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.API.Controllers;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.UserCapture.Identity.API.UnitTests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UsersControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            userServiceMock = mockRepository.Create<IUserService>();
            target = new UsersController(userServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenUserServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UsersController(null));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetUsersListAsync_WhenUsersCollectionExists_ShouldReturnOkResultWithUserListAsync(int groupId)
        {
            var expectedUsers = new List<User> { new User() };

            userServiceMock
                .Setup(userService => userService.GetListAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedUsers);

            var result = await target.GetUsersListAsync(groupId, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<User>>;

            Assert.IsInstanceOfType(apiResponse.Data, expectedUsers.GetType());
            Assert.AreEqual(expectedUsers, apiResponse.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetUserListAsync_WhenUsersNotFound_ShouldReturnOkResultWithNullAsync(int groupId)
        {
            userServiceMock
                .Setup(userService => userService.GetListAsync(groupId, cancellationToken))
                .ReturnsAsync((List<User>)null);

            var result = await target.GetUsersListAsync(groupId, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<User>>;

            Assert.IsNull(apiResponse.Data);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetUserListAsync_WhenTenantIdIsZeroOrNegative_ShouldReturnOkResultWithNullAsync(int groupId)
        {
            var result = await target.GetUsersListAsync(groupId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "groupId parameter has invalid value.");
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetUserByIdAsync_WhenUserFound_ShouldReturnOkResultWithUserAsync(int userId)
        {
            var expectedUser = new User();

            userServiceMock
                .Setup(userService => userService.GetAsync(userId, cancellationToken))
                .ReturnsAsync(expectedUser);

            var result = await target.GetUserByIdAsync(userId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<User>;

            Assert.IsNotNull(apiResponse);
            Assert.IsInstanceOfType(apiResponse.Data, expectedUser.GetType());
            Assert.AreEqual(expectedUser, apiResponse.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetUserByIdAsync_WhenUserNotFound_ShouldReturn404NotFoundResultAsync(int userId)
        {
            userServiceMock
                .Setup(userService => userService.GetAsync(userId, cancellationToken))
                .ReturnsAsync((User)null);

            var result = await target.GetUserByIdAsync(userId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"User with id {userId} not found.");
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetUserByIdAsync_WhenUserIdIsZeroOrNegative_ShouldReturnBadRequestResultAsync(int userId)
        {
            var result = await target.GetUserByIdAsync(userId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "userId parameter has invalid value.");
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType, string expectedMessage)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;

            Assert.IsNotNull(objectResult.Value);
            Assert.IsInstanceOfType(objectResult.Value, typeof(ApiResponse));

            var apiResponse = (ApiResponse)objectResult.Value;
            Assert.AreEqual(expectedMessage, apiResponse.Message);
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
        }

        private Mock<IUserService> userServiceMock;
        private UsersController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
