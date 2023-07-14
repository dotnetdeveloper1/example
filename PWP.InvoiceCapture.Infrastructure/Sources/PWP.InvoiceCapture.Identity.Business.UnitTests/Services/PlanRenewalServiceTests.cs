using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PlanRenewalServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            planServiceMock = mockRepository.Create<IPlanService>();
            groupRepositoryMock = mockRepository.Create<IGroupRepository>();
            groupPlanRepositoryMock = mockRepository.Create<IGroupPlanRepository>();
            target = new PlanRenewalService(groupPlanRepositoryMock.Object, groupRepositoryMock.Object, planServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenPlanServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PlanRenewalService(groupPlanRepositoryMock.Object, groupRepositoryMock.Object, null));
        }

        [TestMethod]
        public void Instance_WhenGroupRepositoryMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PlanRenewalService(groupPlanRepositoryMock.Object, null, planServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenGroupPlanRepositoryMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PlanRenewalService(null, groupRepositoryMock.Object, planServiceMock.Object));
        }

        [TestMethod]
        public async Task CheckAndRenewPlansAsync_WhenPlanIsAboutToExpire_ShouldRenew()
        {
            var expectedGroups = new List<Group>() { new Group { Id = groupId } };
            
            var utcNow = DateTime.UtcNow;
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedGroups);

            groupPlanRepositoryMock
               .Setup(groupPlanRepository => groupPlanRepository.GetActiveAsync(groupId, It.IsAny<DateTime>(), cancellationToken))
               .ReturnsAsync(new GroupPlan() { EndDate = utcNow.Date, PlanId = planId, IsRenewalCancelled = false});
            
            planServiceMock
               .Setup(planService => planService.CreateGroupPlanAsync(groupId, planId, utcNow.Date.AddDays(1), cancellationToken))
               .ReturnsAsync(new OperationResult());

            await target.CheckAndRenewPlansAsync(cancellationToken);
        }

        [TestMethod]
        public async Task CheckAndRenewPlansAsync_WhenPlanIsIsRenewalCancelled_ShouldNotRenew()
        {
            var expectedGroups = new List<Group>() { new Group { Id = groupId } };

            var utcNow = DateTime.UtcNow;
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedGroups);

            groupPlanRepositoryMock
               .Setup(groupPlanRepository => groupPlanRepository.GetActiveAsync(groupId, It.IsAny<DateTime>(), cancellationToken))
               .ReturnsAsync(new GroupPlan() { EndDate = utcNow.Date, PlanId = planId, IsRenewalCancelled = true });

            await target.CheckAndRenewPlansAsync(cancellationToken);
        }

        [TestMethod]
        public async Task CheckAndRenewPlansAsync_WhenDateIsFuture_ShouldNotRenew()
        {
            var expectedGroups = new List<Group>() { new Group { Id = groupId } };

            var utcNow = DateTime.UtcNow;
            var dateToCheck = utcNow.AddDays(3);
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedGroups);

            groupPlanRepositoryMock
               .Setup(groupPlanRepository => groupPlanRepository.GetActiveAsync(groupId, It.IsAny<DateTime>(), cancellationToken))
               .ReturnsAsync(new GroupPlan() { EndDate = dateToCheck, PlanId = planId, IsRenewalCancelled = false });

            await target.CheckAndRenewPlansAsync(cancellationToken);
        }

        [TestMethod]
        public async Task CheckAndRenewPlansAsync_WhenDateIsInPast_ShouldNotRenew()
        {
            var expectedGroups = new List<Group>() { new Group { Id = groupId } };

            var utcNow = DateTime.UtcNow;
            var dateToCheck = utcNow.AddDays(-1);
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedGroups);

            groupPlanRepositoryMock
               .Setup(groupPlanRepository => groupPlanRepository.GetActiveAsync(groupId, It.IsAny<DateTime>(), cancellationToken))
               .ReturnsAsync(new GroupPlan() { EndDate = dateToCheck, PlanId = planId, IsRenewalCancelled = false });

            await target.CheckAndRenewPlansAsync(cancellationToken);
        }

        private MockRepository mockRepository;
        private Mock<IPlanService> planServiceMock;
        private Mock<IGroupRepository> groupRepositoryMock;
        private Mock<IGroupPlanRepository> groupPlanRepositoryMock;
        private PlanRenewalService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int groupId = 12;
        private const int planId = 15;
    }
}
