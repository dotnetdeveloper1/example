using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WebhookServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            hooksRegistrationRepositoryMock = mockRepository.Create<IWebhookRepository>();
            cancellationToken = CancellationToken.None;
            target = new WebhookService(hooksRegistrationRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenWebhookRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new WebhookService(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteAsync_WhenWebhookIdIsZeroOrLessThenZero_ShouldThrowArgumentException(int webhookId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(webhookId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task DeleteAsync_WhenWebhookIdIsCorrect_ShouldDeleteWebhook(int webhookId)
        {           
            hooksRegistrationRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.DeleteAsync(webhookId, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.DeleteAsync(webhookId, cancellationToken);
        }       

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetAsync_WhenWebhookIdIsLessOrEqualsZero_ShouldThrowArgumentException(int webhookId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetAsync(webhookId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(115)]
        public async Task GetAsync_WhenWebhookIdIsCorrect_ShouldReturnWebhook(int webhookId)
        {
            var expectedInvoiceField = new Webhook() { Id = webhookId };

            hooksRegistrationRepositoryMock
                .Setup(hooksRegistrationRepository => hooksRegistrationRepository.GetAsync(webhookId, cancellationToken))
                .ReturnsAsync(expectedInvoiceField);

            var actualInvoiceField = await target.GetAsync(webhookId, cancellationToken);

            Assert.AreEqual(expectedInvoiceField, actualInvoiceField);
        }

        [TestMethod]
        public async Task GetListAsync_WhenWebhooksCollectionExists_ShouldReturnWebhooks()
        {
            var expectedInvoiceFields = new List<Webhook>() { new Webhook(), new Webhook() };

            hooksRegistrationRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedInvoiceFields);

            var actualInvoiceFields = await target.GetListAsync(cancellationToken);

            Assert.AreEqual(expectedInvoiceFields, actualInvoiceFields);
        }

        [TestMethod]
        public void CreateAsync_WhenWebhookIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenWebhookIsNotNull_ShouldCreateWebhook()
        {
            var webhook = new Webhook() { TriggerType = TriggerType.StatusChanged, Url = correctUrl };

            hooksRegistrationRepositoryMock
                .Setup(hooksRegistrationRepository => hooksRegistrationRepository.CreateAsync(webhook, cancellationToken))
                .Returns(Task.CompletedTask);

           hooksRegistrationRepositoryMock
                .Setup(hooksRegistrationRepository => hooksRegistrationRepository.AnyAsync(webhook.TriggerType, webhook.Url, cancellationToken))
                .ReturnsAsync(false);

            var result = await target.CreateAsync(webhook, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public async Task CreateAsync_WhenWebhookUrlIsWrong_ShouldReturnFailedResult()
        {
            var webhook = new Webhook() { TriggerType = TriggerType.StatusChanged, Url = incorrectUrl };

            var result = await target.CreateAsync(webhook, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual("Webhook contains wrong url.", result.Message);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public async Task CreateAsync_WhenWebhookUrlIsEmpty_ShouldReturnFailedResult(string url)
        {
            var webhook = new Webhook() { TriggerType = TriggerType.StatusChanged, Url = url };

            var result = await target.CreateAsync(webhook, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual("Webhook contains empty url.", result.Message);
        }

        [TestMethod]
        [DataRow(1000)]
        [DataRow(-1000)]
        public async Task CreateAsync_WhenTriggerTypeNotDefined_ShouldReturnFailedResult(int triggerType)
        {
            var expectedField = new Webhook() { TriggerType = (TriggerType)triggerType, Url = correctUrl };

            var result = await target.CreateAsync(expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Webhook contains unknown trigger type.", result.Message);
        }

        [TestMethod]
        public async Task CreateAsync_WhenContainsSameWebhook_ShouldReturnFailedResult()
        {
            var webhook = new Webhook() { TriggerType = TriggerType.StatusChanged, Url = correctUrl };

            hooksRegistrationRepositoryMock
                .Setup(hooksRegistrationRepository => hooksRegistrationRepository.AnyAsync(webhook.TriggerType, webhook.Url, cancellationToken))
                .ReturnsAsync(true);

            var result = await target.CreateAsync(webhook, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Web hook with trigger type: {webhook.TriggerType} and url: {webhook.Url} already exists.", result.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenWebhookIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateAsync_WhenWebhookIdIsLessOrEqualsZero_ShouldThrowArgumentException(int webhookId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateAsync(webhookId, new Webhook(), cancellationToken));
        }

        [TestMethod]
        [DataRow(1000)]
        [DataRow(-1000)]
        public async Task UpdateAsync_WhenTriggerTypeNotDefined_ShouldReturnFailedResult(int triggerType)
        {
            var expectedField = new Webhook() { TriggerType = (TriggerType)triggerType, Url = correctUrl };

            var result = await target.UpdateAsync(1, expectedField, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Webhook contains unknown trigger type.", result.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenWebhookIsNotNull_ShouldUpdateWebhook()
        {
            var webhook = new Webhook() { Id = 1, TriggerType = TriggerType.StatusChanged, Url = correctUrl };

            hooksRegistrationRepositoryMock
                .Setup(invoiceFieldRepository => invoiceFieldRepository.UpdateAsync(webhook.Id, webhook, cancellationToken))
                .Returns(Task.CompletedTask);

            hooksRegistrationRepositoryMock
                .Setup(hooksRegistrationRepository => hooksRegistrationRepository.AnyAsync(webhook.TriggerType, webhook.Url, webhook.Id, cancellationToken))
                .ReturnsAsync(false);

            var result = await target.UpdateAsync(webhook.Id, webhook, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenContainsSameWebhook_ShouldReturnFailedResult()
        {
            var webhook = new Webhook() { Id = 1, TriggerType = TriggerType.StatusChanged, Url = correctUrl };

            hooksRegistrationRepositoryMock
                .Setup(hooksRegistrationRepository => hooksRegistrationRepository.AnyAsync(webhook.TriggerType, webhook.Url, webhook.Id, cancellationToken))
                .ReturnsAsync(true);

            var result = await target.UpdateAsync(webhook.Id, webhook, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual($"Web hook with trigger type: {webhook.TriggerType} and url: {webhook.Url} already exists.", result.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenWebhookUrlIsWrong_ShouldReturnFailedResult()
        {
            var webhook = new Webhook() { TriggerType = TriggerType.StatusChanged, Url = incorrectUrl };

            var result = await target.UpdateAsync(1, webhook, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual("Webhook contains wrong url.", result.Message);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public async Task UpdateAsync_WhenWebhookUrlIsEmpty_ShouldReturnFailedResult(string url)
        {
            var webhook = new Webhook() { TriggerType = TriggerType.StatusChanged, Url = url };

            var result = await target.UpdateAsync(1, webhook, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual("Webhook contains empty url.", result.Message);
        }

        private MockRepository mockRepository;
        private Mock<IWebhookRepository> hooksRegistrationRepositoryMock;
        private WebhookService target;
        private CancellationToken cancellationToken;

        private readonly string correctUrl = "http://www.correcturl.com";
        private readonly string incorrectUrl = "erwerwe";
    }
}
