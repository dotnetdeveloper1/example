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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.API.UnitTests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class GroupsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            groupServiceMock = mockRepository.Create<IGroupService>();
            target = new GroupsController(groupServiceMock.Object);
        }

        [TestMethod]
        public async Task GetGroupsListAsync_WhenGroupsCollectionExists_ShouldReturnOkResultWithGroupsListAsync()
        {
            var expectedGroups = new List<Group> { new Group() };

            groupServiceMock
                .Setup(groupService => groupService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedGroups);

            var result = await target.GetGroupsListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponse = result.Value as ApiResponse<List<Group>>;

            Assert.IsInstanceOfType(apiResponse.Data, expectedGroups.GetType());
            Assert.AreEqual(expectedGroups, apiResponse.Data);
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
        }

        private Mock<IGroupService> groupServiceMock;
        private GroupsController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
