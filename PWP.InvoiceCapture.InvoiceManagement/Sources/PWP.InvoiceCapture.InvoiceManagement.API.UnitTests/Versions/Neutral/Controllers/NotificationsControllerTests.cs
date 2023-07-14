using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.API.Versions.Neutral.Controllers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.API.UnitTests.Versions.Neutral.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class NotificationsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            webhookServiceMock = mockRepository.Create<IWebhookService>();
            target = new NotificationsController(webhookServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenWebhookServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NotificationsController(null));
        }

        [TestMethod]
        public async Task GetWebhooksListAsync_WhenWebhooksCollectionExists_ShouldReturnOkResultWithWebhooksList()
        {
            var expectedHooks = new List<Webhook> { new Webhook() };

            webhookServiceMock
                .Setup(webhookService => webhookService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedHooks);

            var result = await target.GetWebhooksListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<Webhook>>;

            Assert.IsInstanceOfType(apiResponce.Data, expectedHooks.GetType());
            Assert.AreEqual(expectedHooks, apiResponce.Data);
        }

        [TestMethod]
        public async Task GetWebhookListAsync_WhenWebhooksWereFound_ShouldReturnOkResult()
        {
            var expectedHooks = new List<Webhook>() { new Webhook() };

            webhookServiceMock
                .Setup(webhookService => webhookService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedHooks);

            var result = await target.GetWebhooksListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<Webhook>>;

            Assert.IsNotNull(apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetWebhookByIdAsync_WhenWebhookFound_ShouldReturnOkResultWithField(int webhookId)
        {
            var webhook = new Webhook();

            webhookServiceMock
                .Setup(webhookService => webhookService.GetAsync(webhookId, cancellationToken))
                .ReturnsAsync(webhook);

            var result = await target.GetWebhookByIdAsync(webhookId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<Webhook>;

            Assert.IsNotNull(apiResponce);
            Assert.IsInstanceOfType(apiResponce.Data, webhook.GetType());
            Assert.AreEqual(webhook, apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetWebhookByIdAsync_WhenWebhookNotFound_ShouldReturn404NotFoundResult(int webhookId)
        {
            webhookServiceMock
                .Setup(webhookService => webhookService.GetAsync(webhookId, cancellationToken))
                .ReturnsAsync((Webhook)null);

            var result = await target.GetWebhookByIdAsync(webhookId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Webhook registration request with id = {webhookId} not found.");
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetWebhookByIdAsyncc_WhenWebhookIdParameterOutOfRange_ShouldReturnBadRequestResult(int webhookId)
        {
            var result = await target.GetWebhookByIdAsync(webhookId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(webhookId)} parameter has invalid value.");
        }

        [TestMethod]
        public async Task CreateAsync_WhenWebhookExists_ShouldReturnOkObjectResult()
        {
            var webhookToSave = new Webhook();

            webhookServiceMock
                .Setup(webhookService => webhookService.CreateAsync(webhookToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Success });

            var result = await target.CreateWebhookAsync(webhookToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task CreateAsync_WhenWebhookHasWrongTriggerType_ShouldReturnBadRequestObjectResult()
        {
            var webhookToSave = new Webhook() { TriggerType = (TriggerType)1000 };
            var message = $"Webhook registration request contains unknown trigger type.";

            webhookServiceMock
                .Setup(webhookService => webhookService.CreateAsync(webhookToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Failed, Message = message });

            var result = await target.CreateWebhookAsync(webhookToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateWebhookAsync_WhenWebhookExists_ShouldReturnOkObjectResult(int webhookId)
        {
            var webhookToSave = new Webhook() { Id = webhookId };
            var message = $"Webhook registration request with id = {webhookId} was updated.";

            webhookServiceMock
                .Setup(webhookService => webhookService.UpdateAsync(webhookId, webhookToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Success, Message = message });

            var result = await target.UpdateAsync(webhookId, webhookToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateWebhookAsync_WhenWebhookHasWrongTriggerType_ShouldReturnBadRequestObjectResult(int webhookId)
        {
            var webhookToSave = new Webhook() { Id = webhookId, TriggerType = (TriggerType)1000 };
            var message = $"Webhook registration request contains unknown trigger type.";

            webhookServiceMock
                .Setup(webhookService => webhookService.UpdateAsync(webhookId, webhookToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.Failed, Message = message });

            var result = await target.UpdateAsync(webhookId, webhookToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult), message);
        }

        [DataRow(1)]
        [DataRow(2)]
        public async Task UpdateWebhookAsync_WhenWebhookNotExists_ShouldReturnBadRequestObjectResult(int webhookId)
        {
            var webhookToSave = new Webhook() { Id = webhookId };
            var message = $"Webhook registration request with id = {webhookId} not found.";

            webhookServiceMock
                .Setup(webhookService => webhookService.GetAsync(webhookId, cancellationToken))
                .ReturnsAsync(webhookToSave);

            webhookServiceMock
                .Setup(webhookService => webhookService.UpdateAsync(webhookId, webhookToSave, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = OperationResultStatus.NotFound, Message = message });

            var result = await target.UpdateAsync(webhookId, webhookToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-10)]
        public async Task UpdateWebhookAsync_WhenWebhookIdIsLessOrEqualstoZeroNotFound_ShouldReturnBadRequestObjectResult(int webhookId)
        {
            var webhookToSave = new Webhook() { Id = webhookId };

            var result = await target.UpdateAsync(webhookId, webhookToSave, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(webhookId)} parameter has invalid value.");
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task DeleteAsync_WhenWebhookWasRemoved_ShouldReturnOkObjectResult(int webhookId)
        {
            var webhookToSave = new Webhook() { Id = webhookId };
            var message = $"Webhook registration request with id = {webhookId} was removed.";

            webhookServiceMock
                .Setup(webhookService => webhookService.DeleteAsync(webhookId, cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await target.DeleteAsync(webhookId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-10)]
        public async Task DeleteAsync_WhenWebhookIdIsLessOrEqualsToZero_ShouldReturnBadRequestObjectResult(int webhookId)
        {
            var result = await target.DeleteAsync(webhookId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(webhookId)} parameter has invalid value.");
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

        private Mock<IWebhookService> webhookServiceMock;
        private NotificationsController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
