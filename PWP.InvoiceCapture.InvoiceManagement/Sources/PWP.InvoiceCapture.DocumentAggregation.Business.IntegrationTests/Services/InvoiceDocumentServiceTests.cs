using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Document.API.Client;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Options;
using PWP.InvoiceCapture.DocumentAggregation.Business.IntegrationTests.Fakes;
using PWP.InvoiceCapture.DocumentAggregation.Business.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.IntegrationTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceDocumentServiceTests
    {
        [TestInitialize]
        public async Task Initialize()
        {
            sync = new SemaphoreSlim(1, 1);
            messageHandler = new FakeMessageHandler(sync);
            exceptionHandler = new FakeExceptionHandler();
            subscriber = CreateSubscriber();
            publisher = CreatePublisher();
            documentApiClient = CreateDocumentApiClient();
            applicationContext = new FakeApplicationContext();
            target = new InvoiceDocumentService(documentApiClient, publisher, applicationContext);

            await subscriber.StartAsync(cancellationToken);
            await publisher.StartAsync(cancellationToken);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            sync.Release();

            await publisher.StopAsync(cancellationToken);
            await subscriber.StopAsync(cancellationToken);
        }

        [TestMethod]
        [DataRow(0)]    
        [DataRow(1024)]         //   1 Kb
        [DataRow(1048576)]      //   1 Mb
        [DataRow(10485760)]     //  10 Mb
        [DataRow(104857600)]    // 100 Mb
        public async Task SaveAsync_ShouldUploadFileAndPublishServiceBusMessage(int fileSizeBytes) 
        {
            var expectedFileBytes = new byte[fileSizeBytes];
            random.NextBytes(expectedFileBytes);

            using (var fileStream = new MemoryStream(expectedFileBytes))
            {
                var uploadedDocument = await target.SaveAsync(fileStream, fileName, sourceType, cancellationToken);
                Assert.IsNotNull(uploadedDocument);
            }

            await sync.WaitAsync(serviceBusWaitingTimeoutInMilliseconds, cancellationToken);

            Assert.AreEqual(1, messageHandler.HandledMessages.Count);

            var innerMessage = messageHandler.HandledMessages.Single().InnerMessage;
            Assert.IsInstanceOfType(innerMessage, typeof(InvoiceDocumentUploadedMessage));

            var actualMessage = (InvoiceDocumentUploadedMessage)innerMessage;

            Assert.AreEqual(fileName, actualMessage.FileName);
            Assert.AreEqual(sourceType, actualMessage.FileSourceType);

            byte[] actualFileBytes = null;

            using (var fileStream = await documentApiClient.GetDocumentStreamAsync(actualMessage.FileId, cancellationToken))
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                actualFileBytes = memoryStream.ToArray();
            }

            Assert.AreEqual(fileSizeBytes, actualFileBytes.Length);
            CollectionAssert.AreEqual(expectedFileBytes, actualFileBytes);
        }

        private IServiceBusSubscriber CreateSubscriber() 
        {
            var subscriberOptions = new ServiceBusSubscriberOptions 
            {
                ConnectionString = connectionString,
                TopicName = topicName,
                SubscriberName = subscriberName,
                MaxConcurrentCalls = 1
            };

            var messageHandlers = new List<IMessageHandler> { messageHandler };

            return new ServiceBusSubscriberFactory().Create(messageHandlers, exceptionHandler, subscriberOptions);
        }

        private IServiceBusPublisher CreatePublisher() 
        {
            var publisherOptions = new ServiceBusPublisherOptions 
            {
                ConnectionString = connectionString,
                TopicName = topicName
            };

            return new ServiceBusPublisherFactory().Create(publisherOptions);
        }

        private IDocumentApiClient CreateDocumentApiClient() 
        {
            var documentApiOptions = new DocumentApiClientOptions
            {
                BaseAddress = documentApiBaseAddress,
                RetryAttemptCount = 5,
                TimeoutInSeconds = serviceBusWaitingTimeoutInMilliseconds / 1000
            };

            return new DocumentApiClient(Options.Create(documentApiOptions));
        }

        private FakeExceptionHandler exceptionHandler = new FakeExceptionHandler();
        private FakeMessageHandler messageHandler;
        private SemaphoreSlim sync;
        private IServiceBusPublisher publisher;
        private IServiceBusSubscriber subscriber;
        private IDocumentApiClient documentApiClient;
        private InvoiceDocumentService target;
        private IApplicationContext applicationContext;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private readonly FileSourceType sourceType = FileSourceType.API;
        private readonly Random random = new Random();
        private readonly string fileName = "invoice.pdf";
        private readonly string documentApiBaseAddress = "http://invoicecapture-dev-backend.eastus.cloudapp.azure.com:10083";
        private readonly string connectionString = "Endpoint=sb://invoicebuso27h3yhrdfv74.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=qF7r1IOVaaiBQW9zMtD2zxB9Dkvt0+HqFVHU9HxjI+8=";
        private readonly string topicName = "DocumentAggregation.Business.IntegrationTests";
        private readonly string subscriberName = "DocumentAggregation.Business.IntegrationTests";
        private readonly int serviceBusWaitingTimeoutInMilliseconds = 900000;
    }
}
