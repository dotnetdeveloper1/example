using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
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
    public class UsageServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            tenantServiceMock = mockRepository.Create<ITenantService>();
            planServiceMock = mockRepository.Create<IPlanService>();
            packServiceMock = mockRepository.Create<IPackService>();
            target = new UsageService(tenantServiceMock.Object, planServiceMock.Object, packServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenTenantServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UsageService(null, planServiceMock.Object, packServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenPlanServiceMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UsageService(tenantServiceMock.Object, null, packServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenPackServiceMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UsageService(tenantServiceMock.Object, planServiceMock.Object, null));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task IncreaseCountOfUploadedInvoicesAsync_WhenTenantIdIsWrong_ShouldThrowArgumentExceptionAsync(int tenantId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.TryIncreaseCountOfUploadedInvoicesAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task IncreaseCountOfUploadedInvoicesAsync_WhenGroupIdIsWrong_ShouldThrowArgumentExceptionAsync(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetUsageAsync(groupId, cancellationToken));
        }

        [TestMethod]
        public async Task GetUsageAsync_ShouldReturnUsage()
        {
            var expectedGroupPlan = new GroupPlan { Id = groupPlanId };
            var expectedGroupPacks = new List<GroupPack>() { new GroupPack { Id = groupPackId } };

            planServiceMock
                .Setup(planService => planService.GetActiveAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedGroupPlan);

            packServiceMock
                .Setup(packService => packService.GetActiveGroupPackAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedGroupPacks);

            var result = await target.GetUsageAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(groupPackId, result.ActivePlan.Id, groupPackId);
            Assert.AreEqual(groupPackId, result.TotalAvailablePacks[0].Id);
        }

        [TestMethod]
        public async Task IncreaseCountOfUploadedInvoicesAsync_WhenThereIsActivePlan_ShouldIncreasePlanUsage()
        {
            var expectedGroupPlan = new GroupPlan
            {
                Id = groupPlanId,
                UploadedDocumentsCount = 5,
                Plan = new Plan()
                {
                    AllowedDocumentsCount = 10
                }
            };

            var expectedGroupPacks = new List<GroupPack>() { new GroupPack { Id = groupPackId } };

            planServiceMock
                .Setup(planService => planService.LockGroupPlansAsync(groupId, cancellationToken))
                .Returns(Task.CompletedTask);

            planServiceMock
                .Setup(planService => planService.GetActiveAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedGroupPlan);

            planServiceMock
                .Setup(planService => planService.IncreaseCountOfUploadedInvoices(It.IsAny<GroupPlan>(), cancellationToken))
                .Returns(Task.CompletedTask);
           
            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(new Tenant()
                {
                    GroupId = groupId
                });

            var limitNotExceeded = await target.TryIncreaseCountOfUploadedInvoicesAsync(tenantId, cancellationToken);

            Assert.IsTrue(limitNotExceeded);
        }

        [TestMethod]
        public async Task IncreaseCountOfUploadedInvoicesAsync_WhenThereIsActivePlanWithNoDocumentsLeft_ShouldNotIncreasePlanUsage()
        {
            var expectedGroupPlan = new GroupPlan
            {
                Id = groupPlanId,
                UploadedDocumentsCount = 5,
                Plan = new Plan()
                {
                    AllowedDocumentsCount = 5
                }
            };

            var expectedGroupPacks = new List<GroupPack>() { new GroupPack { Id = groupPackId } };

            planServiceMock
                .Setup(planService => planService.LockGroupPlansAsync(groupId, cancellationToken))
                .Returns(Task.CompletedTask);

            planServiceMock
                .Setup(planService => planService.GetActiveAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedGroupPlan);

            packServiceMock
                .Setup(packService => packService.LockGroupPacksAsync(groupId, cancellationToken))
                .Returns(Task.CompletedTask);

            packServiceMock
                .Setup(packService => packService.GetActiveGroupPackAsync(groupId, cancellationToken))
                .ReturnsAsync(new List<GroupPack>());
           
            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(new Tenant()
                {
                    GroupId = groupId
                });

            var limitNotExceeded = await target.TryIncreaseCountOfUploadedInvoicesAsync(tenantId, cancellationToken);

            Assert.IsFalse(limitNotExceeded);
        }

        [TestMethod]
        public async Task IncreaseCountOfUploadedInvoicesAsync_WhenThereIsNoActivePlanAndThereIsActivePack_ShouldIncreasePackUsage()
        {
            var expectedGroupPlan = new GroupPlan
            {
                Id = groupPlanId,
                UploadedDocumentsCount = 5,
                Plan = new Plan()
                {
                    AllowedDocumentsCount = 5
                }
            };

            var expectedGroupPacks = new List<GroupPack>() { new GroupPack { Id = groupPackId } };

            planServiceMock
                .Setup(planService => planService.LockGroupPlansAsync(groupId, cancellationToken))
                .Returns(Task.CompletedTask);

            planServiceMock
                .Setup(planService => planService.GetActiveAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedGroupPlan);

            packServiceMock
                .Setup(packService => packService.LockGroupPacksAsync(groupId, cancellationToken))
                .Returns(Task.CompletedTask);

            packServiceMock
                .Setup(packService => packService.GetActiveGroupPackAsync(groupId, cancellationToken))
                .ReturnsAsync(new List<GroupPack>() { new GroupPack() { } });

            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(new Tenant() { GroupId = groupId });

            packServiceMock
               .Setup(packService => packService.IncreaseCountOfUploadedInvoices(It.IsAny<GroupPack>(), cancellationToken))
               .Returns(Task.CompletedTask);

            var limitNotExceeded = await target.TryIncreaseCountOfUploadedInvoicesAsync(tenantId, cancellationToken);

            Assert.IsTrue(limitNotExceeded);
        }

        private MockRepository mockRepository;
        private Mock<IPlanService> planServiceMock;
        private Mock<IPackService> packServiceMock;
        private Mock<ITenantService> tenantServiceMock;
        private UsageService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int groupId = 16;
        private const int groupPlanId = 11;
        private const int groupPackId = 12;
        private const int tenantId = 120;
    }
}
