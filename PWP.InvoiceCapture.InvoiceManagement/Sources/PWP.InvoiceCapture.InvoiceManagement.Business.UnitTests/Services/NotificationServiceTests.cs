using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Notification;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class NotificationServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            webhookServiceMock = mockRepository.Create<IWebhookService>();
            notificationClientMock = mockRepository.Create<INotificationClient>();
            notificationClientFactoryMock = mockRepository.Create<INotificationClientFactory>();
            cancellationToken = CancellationToken.None;
            target = new NotificationService(webhookServiceMock.Object, notificationClientFactoryMock.Object, CreateOptions());
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenWebhookServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NotificationService(
                null,
                notificationClientFactoryMock.Object,
                CreateOptions()));
        }

        [TestMethod]
        public void Instance_WhenNotificationClientFactoryMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NotificationService(
                webhookServiceMock.Object,
                null,
                CreateOptions()));
        }

        [TestMethod]
        public void Instance_WhenOptionsIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NotificationService(
                webhookServiceMock.Object,
                notificationClientFactoryMock.Object,
                null));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenOptionsInvoiceUrlIsNullOrWhitespace_ShouldThrowArgumentNullException(string url)
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NotificationService(
                webhookServiceMock.Object,
                notificationClientFactoryMock.Object,
                CreateOptions(url)
                ));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task NotifyAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.NotifyAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        public async Task NotifyAsync_WhenNoHooks_ShouldReturn()
        {
            webhookServiceMock
                .Setup(webhookService => webhookService.GetListAsync(cancellationToken))
                .ReturnsAsync((List<Webhook>)null);

            await target.NotifyAsync(1, cancellationToken);
        }

        [TestMethod]
        public async Task NotifyAsync_WhenNoNeededHooks_ShouldReturn()
        {
            webhookServiceMock
                .Setup(webhookService => webhookService.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Webhook>() { new Webhook() { TriggerType = 0 } });

            await target.NotifyAsync(1, cancellationToken);
        }

        [TestMethod]
        [DataRow(11)]
        [DataRow(112)]
        public async Task NotifyAsync__ShouldNotify(int invoiceId)
        {
            var actualNotification = new List<WebhookNotification>();
            
            webhookServiceMock
                .Setup(webhookService => webhookService.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Webhook>() { new Webhook() { TriggerType = TriggerType.StatusChanged, Url = tenantHookUrl } });
           
            notificationClientMock
               .Setup(notificationClient => notificationClient.NotifyAsync(Capture.In(actualNotification), cancellationToken))
               .Returns(Task.CompletedTask);

            notificationClientFactoryMock
                .Setup(notificationClientFactory => notificationClientFactory.Create(tenantHookUrl))
                .Returns(notificationClientMock.Object);

            await target.NotifyAsync(invoiceId, cancellationToken);

            Assert.AreEqual(1, actualNotification.Count);
            Assert.AreEqual(TriggerType.StatusChanged, actualNotification[0].TriggerType);
            Assert.AreEqual(invoiceUrlPattern.Replace("{id}", invoiceId.ToString()), actualNotification[0].InvoiceUrl);
        }

        private IOptions<NotificationOptions> CreateOptions(string url = invoiceUrlPattern)
        {
            return Options.Create<NotificationOptions>(new NotificationOptions()
            {
               InvoiceUrlPattern = url
            });
        }

        private MockRepository mockRepository;
        private Mock<IWebhookService> webhookServiceMock;
        private Mock<INotificationClientFactory> notificationClientFactoryMock;
        private Mock<INotificationClient> notificationClientMock;
        private NotificationService target;
        private CancellationToken cancellationToken;

        private const string invoiceUrlPattern = "somePattern{id}";
        private const string tenantHookUrl = "someurl";
    }
}
