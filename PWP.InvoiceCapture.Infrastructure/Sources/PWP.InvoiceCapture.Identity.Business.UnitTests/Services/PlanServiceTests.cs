using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PlanServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            planRepositoryMock = mockRepository.Create<IPlanRepository>();
            groupRepositoryMock = mockRepository.Create<IGroupRepository>();
            groupPlanRepositoryMock = mockRepository.Create<IGroupPlanRepository>();
            currencyRepositoryMock = mockRepository.Create<ICurrencyRepository>();
            target = new PlanService(planRepositoryMock.Object, groupPlanRepositoryMock.Object, groupRepositoryMock.Object, currencyRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenPlanRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PlanService(null, groupPlanRepositoryMock.Object, groupRepositoryMock.Object, currencyRepositoryMock.Object));
        }

        [TestMethod]
        public void Instance_WhenGroupRepositoryMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PlanService(planRepositoryMock.Object, groupPlanRepositoryMock.Object, null, currencyRepositoryMock.Object));
        }

        [TestMethod]
        public void Instance_WhenGroupPlanRepositoryMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PlanService(planRepositoryMock.Object, null, groupRepositoryMock.Object, currencyRepositoryMock.Object));
        }

        [TestMethod]
        public void Instance_WhenCurrencyRepositoryMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PlanService(planRepositoryMock.Object, groupPlanRepositoryMock.Object, groupRepositoryMock.Object, null));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetGroupPlanListAsync_WhenGroupIdIsWrong_ShouldThrowArgumentException(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetGroupPlanListAsync(groupId, cancellationToken));
        }

        [TestMethod]
        public async Task GetGroupPlanList_WhenPlanCollectionExists_ShouldReturnPlans()
        {
            var expectedPlans = new List<GroupPlan>() { new GroupPlan { Id = groupPlanId } };

            groupPlanRepositoryMock
                .Setup(groupPlanRepository => groupPlanRepository.GetListAsync(groupId, cancellationToken))
                .ReturnsAsync(expectedPlans);

            var actualPlans = await target.GetGroupPlanListAsync(groupId, cancellationToken);

            Assert.IsNotNull(actualPlans);
            Assert.AreEqual(actualPlans.Count(), 1);
            Assert.AreEqual(groupPlanId, actualPlans[0].Id);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task CreateGroupPlanAsync_WhenGroupIdIsWrong_ShouldThrowArgumentException(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.CreateGroupPlanAsync(groupId, 1, DateTime.Now, cancellationToken));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task CreateGroupPlanAsync_WhenPlanIdIsWrong_ShouldThrowArgumentException(int planId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.CreateGroupPlanAsync(1, planId, DateTime.Now, cancellationToken));
        }

        [TestMethod]
        public async Task CreateGroupPlanAsync_WhenPlanIsNotExists_ShouldReturnFailedOperationResultWithCorrespondingMessage()
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(true);
            planRepositoryMock
                .Setup(planRepository => planRepository.GetByIdAsync(planId, cancellationToken))
                .ReturnsAsync((Plan)null);

            var result = await target.CreateGroupPlanAsync(groupId, planId, DateTime.Now, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"Plan with id={planId} was not found", result.Message);
        }

        [TestMethod]
        public async Task CreateGroupPlanAsync_WhenGroupIdIsNotExists_ShouldReturnFailedOperationResultWithCorrespondingMessage()
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(false);

            var result = await target.CreateGroupPlanAsync(groupId, planId, DateTime.Now, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"Group with id={groupId} was not found", result.Message);

        }

        [TestMethod]
        public async Task CreateGroupPlanAsync_WhenTimeIsNotSetUp_ShouldReturnFailedOperationResultWithCorrespondingMessage()
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(true);
            planRepositoryMock
                .Setup(planRepository => planRepository.GetByIdAsync(planId, cancellationToken))
                .ReturnsAsync(new Plan());

            var result = await target.CreateGroupPlanAsync(groupId, planId, new DateTime(), cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"StartDate was not assigned", result.Message);
        }

        [DataRow(PlanType.Monthly)]
        [DataRow(PlanType.Annual)]
        [TestMethod]
        public async Task CreateGroupPlanAsync_MonthlyAndAnnual_ShouldCreate(PlanType planType)
        {
            groupRepositoryMock
                .Setup(groupRepository => groupRepository.ExistsAsync(groupId, cancellationToken))
                .ReturnsAsync(true);

            planRepositoryMock
                .Setup(planRepository => planRepository.GetByIdAsync(planId, cancellationToken))
                .ReturnsAsync(new Plan() { Type = planType });

            var startDate = DateTime.Parse("2020-03-04", CultureInfo.InvariantCulture);
            var expectedEndDate = new DateTime();

            if (planType == PlanType.Monthly)
            {
                expectedEndDate = startDate.AddMonths(1);
            }

            if (planType == PlanType.Annual)
            {
                expectedEndDate = startDate.AddYears(1);
            }

            groupPlanRepositoryMock
                .Setup(planRepository => planRepository.IsIntersectAsync(groupId, startDate, expectedEndDate, cancellationToken))
                .ReturnsAsync(false);

            var actualGroupPlan = new List<GroupPlan>();

            groupPlanRepositoryMock
                .Setup(planRepository => planRepository.CreateAsync(Capture.In(actualGroupPlan), cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await target.CreateGroupPlanAsync(groupId, planId, DateTime.Parse("2020-03-04", CultureInfo.InvariantCulture), cancellationToken);

            Assert.AreEqual(startDate, actualGroupPlan[0].StartDate);
            Assert.AreEqual(expectedEndDate, actualGroupPlan[0].EndDate);
            Assert.AreEqual(planId, actualGroupPlan[0].PlanId);
            Assert.AreEqual(groupId, actualGroupPlan[0].GroupId);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldCallRepository()
        {
            planRepositoryMock
                 .Setup(planRepository => planRepository.GetByIdAsync(planId, cancellationToken))
                 .ReturnsAsync(new Plan() { });

            var result = await target.GetByIdAsync(planId, cancellationToken);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetActiveAsync_ShouldCallRepository()
        {
            groupPlanRepositoryMock
                 .Setup(groupPlanRepository => groupPlanRepository.GetActiveAsync(groupId, It.IsAny<DateTime>(), cancellationToken))
                 .ReturnsAsync(new GroupPlan());

            var result = await target.GetActiveAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task IncreaseCountOfUploadedInvoicesAsync_ShouldIncrease()
        {
            var groupPlans = new List<GroupPlan>() { };
            groupPlanRepositoryMock
                 .Setup(groupPlanRepository => groupPlanRepository.UpdateAsync(Capture.In(groupPlans), cancellationToken))
                 .Returns(Task.CompletedTask);
            var groupPlanToUpdate = new GroupPlan()
            {
                Id = groupPlanId,
                UploadedDocumentsCount = 0
            };
            await target.IncreaseCountOfUploadedInvoices(groupPlanToUpdate, cancellationToken);

            Assert.AreEqual(1, groupPlans.Count);
            Assert.AreEqual(1, groupPlans[0].UploadedDocumentsCount);
        }

        [TestMethod]
        public async Task GetListAsync_ShouldCallRepository()
        {
            planRepositoryMock
                 .Setup(planRepository => planRepository.GetListAsync(cancellationToken))
                 .ReturnsAsync(new List<Plan>() { });

            var result = await target.GetPlanListAsync(cancellationToken);

            Assert.IsNotNull(result);
        }

        [DataRow(null)]
        [DataRow("")]
        [TestMethod]
        public async Task CreateAsync_WhenPlanNameIsEmpty_ShouldReturnFailedOperationResultWithCorrespondingMessage(string planName)
        {
            var result = await target.CreatePlanAsync(new PlanCreationParameters() { PlanName = planName }, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"Name required.", result.Message);
        }

        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(22)]
        [TestMethod]
        public async Task CreateAsync_WhenPlanTypeIsWrong_ShouldReturnFailedOperationResultWithCorrespondingMessage(int typeId)
        {
            var result = await target.CreatePlanAsync(new PlanCreationParameters() { PlanName = testPlanName, TypeId = typeId }, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"Plan type is incorrect.", result.Message);
        }

        [TestMethod]
        public async Task CreateAsync_WhenAllowedDocumentsCountIsNegative_ShouldReturnFailedOperationResultWithCorrespondingMessage()
        {
            var result = await target.CreatePlanAsync(
                new PlanCreationParameters() { PlanName = testPlanName, TypeId = testPlanType, AllowedDocumentsCount = -1 }, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"AllowedDocumentsCount is less then zero.", result.Message);
        }

        [TestMethod]
        public async Task CreateAsync_WhenCurrencyIsNotExists_ShouldReturnFailedOperationResultWithCorrespondingMessage()
        {
            currencyRepositoryMock
                .Setup(currencyRepository => currencyRepository.ExistsAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(false);

            var result = await target.CreatePlanAsync(
                new PlanCreationParameters() { PlanName = testPlanName, TypeId = testPlanType, AllowedDocumentsCount = allowedDocumentsCount, CurrencyId = currencyId }, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual($"Currency with id {currencyId} is not exists.", result.Message);
        }

        [TestMethod]
        public async Task CreateAsync_WhenParamsAreOk_ShouldCallRepository()
        {
            var actualPlans = new List<Plan>();
            currencyRepositoryMock
                 .Setup(currencyRepository => currencyRepository.ExistsAsync(It.IsAny<int>(), cancellationToken))
                 .ReturnsAsync(true);
            
            planRepositoryMock
                 .Setup(planRepository => planRepository.CreateAsync(Capture.In(actualPlans), cancellationToken))
                 .ReturnsAsync(It.IsAny<int>());

            var result = await target.CreatePlanAsync(
                new PlanCreationParameters() { PlanName = testPlanName, TypeId = testPlanType, AllowedDocumentsCount = allowedDocumentsCount, CurrencyId = currencyId, Price = price}, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccessful);
            Assert.AreEqual(1, actualPlans.Count);
            var actualPlan = actualPlans[0];
            Assert.AreEqual(testPlanName, actualPlan.Name);
            Assert.AreEqual(allowedDocumentsCount, actualPlan.AllowedDocumentsCount);
            Assert.AreEqual(price, actualPlan.Price);
            Assert.AreEqual(PlanType.Monthly, actualPlan.Type);
        }

        [DataRow(2)]
        [DataRow(15)]
        [TestMethod]
        public async Task DeleteActiveAsync_ShouldDelete(int groupId)
        {
            var groupPlanId = 5;
            groupPlanRepositoryMock
                 .Setup(groupPlan => groupPlan.GetActiveAsync(groupId, It.IsAny<DateTime>(), cancellationToken))
                 .ReturnsAsync(new GroupPlan() { Id = groupPlanId });

            groupPlanRepositoryMock
                 .Setup(groupPlan => groupPlan.DeleteAsync(groupPlanId, cancellationToken))
                 .Returns(Task.CompletedTask);

            var result = await target.DeleteActiveAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccessful);
        }

        [DataRow(2)]
        [DataRow(15)]
        [TestMethod]
        public async Task DeleteActiveAsync_WhenThereIsNoActivePlan_ShouldReturnNotFoundResult(int groupId)
        {
            groupPlanRepositoryMock
                 .Setup(groupPlan => groupPlan.GetActiveAsync(groupId, It.IsAny<DateTime>(), cancellationToken))
                 .ReturnsAsync((GroupPlan)null);

            var result = await target.DeleteActiveAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.NotFound, result.Status);
            Assert.AreEqual($"There is no active plan now for groupId = '{groupId}'.", result.Message);
        }

        [DataRow(3)]
        [DataRow(16)]
        [TestMethod]
        public async Task DeleteGroupPlanByIdAsync_ShouldDelete(int groupPlanId)
        {
            groupPlanRepositoryMock
                 .Setup(groupPlan => groupPlan.GetByIdAsync(groupPlanId, cancellationToken))
                 .ReturnsAsync(new GroupPlan() { Id = groupPlanId });

            groupPlanRepositoryMock
                 .Setup(groupPlan => groupPlan.DeleteAsync(groupPlanId, cancellationToken))
                 .Returns(Task.CompletedTask);

            var result = await target.DeleteGroupPlanByIdAsync(groupPlanId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccessful);
        }

        [DataRow(3)]
        [DataRow(16)]
        [TestMethod]
        public async Task DeleteGroupPlanByIdAsync_WhenThereIsNoGroupPlan_ShouldReturnNotFoundResult(int groupPlanId)
        {
            groupPlanRepositoryMock
                 .Setup(groupPlan => groupPlan.GetByIdAsync(groupPlanId, cancellationToken))
                 .ReturnsAsync((GroupPlan)null);

            var result = await target.DeleteGroupPlanByIdAsync(groupPlanId, cancellationToken);
           
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.NotFound, result.Status);
            Assert.AreEqual($"There is no groupPlan with id = '{groupPlanId}'.", result.Message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task DeleteGroupPlanByIdAsync_WhenGroupPlanIdIsWrong_ShouldThrowArgumentException(int groupPlanId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteGroupPlanByIdAsync(groupPlanId, cancellationToken));
        }

        [DataRow(2)]
        [DataRow(15)]
        [TestMethod]
        public async Task GetActiveAsync_WhenThereIsNoActivePlan_ShouldReturnNotFoundResult(int groupId)
        {
            groupPlanRepositoryMock
                 .Setup(groupPlan => groupPlan.GetActiveAsync(groupId, It.IsAny<DateTime>(), cancellationToken))
                 .ReturnsAsync((GroupPlan)null);

            var result = await target.CancelRenewalAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.NotFound, result.Status);
            Assert.AreEqual($"There is no active plan now for groupId = '{groupId}'.", result.Message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task CancelRenewalAsync_WhenGroupIdIsWrong_ShouldThrowArgumentException(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.CancelRenewalAsync(groupId, cancellationToken));
        }

        [DataRow(3)]
        [DataRow(16)]
        [TestMethod]
        public async Task CancelRenewalAsync_ShouldCancelRenewal(int groupId)
        {
            var activePlanId = 9;
            groupPlanRepositoryMock
                 .Setup(groupPlan => groupPlan.GetActiveAsync(groupId, It.IsAny<DateTime>(), cancellationToken))
                 .ReturnsAsync(new GroupPlan() { Id = activePlanId });

            groupPlanRepositoryMock
                 .Setup(groupPlan => groupPlan.CancelRenewalAsync(activePlanId, cancellationToken))
                 .Returns(Task.CompletedTask);

            var result = await target.CancelRenewalAsync(groupId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccessful);
        }

        private MockRepository mockRepository;
        private Mock<IPlanRepository> planRepositoryMock;
        private Mock<IGroupRepository> groupRepositoryMock;
        private Mock<IGroupPlanRepository> groupPlanRepositoryMock;
        private Mock<ICurrencyRepository> currencyRepositoryMock;
        private PlanService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int groupId = 12;
        private const int planId = 15;
        private const int groupPlanId = 25;
        private const string testPlanName = "somePlanName";
        private const int testPlanType = 1;
        private const int currencyId = 5;
        private const int allowedDocumentsCount = 55;
        private const decimal price = 66.78m;
    }
}
