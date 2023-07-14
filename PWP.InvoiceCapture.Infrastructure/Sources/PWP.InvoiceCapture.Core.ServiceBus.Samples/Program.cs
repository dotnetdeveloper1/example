using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Samples
{
    public class Program
    {
        static async Task Main()
        {
            // Publish Messages using ServiceBusPublisher
            //--------------------------------------------

            var serviceBusPublisherOptions = new ServiceBusPublisherOptions
            {
                ConnectionString = "Endpoint=sb://invoicebuso27h3yhrdfv74.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=qF7r1IOVaaiBQW9zMtD2zxB9Dkvt0+HqFVHU9HxjI+8=",
                TopicName = "InvoiceManagement.Sample"
            };

            var publisher = new ServiceBusPublisherFactory().Create(serviceBusPublisherOptions);

            await publisher.StartAsync(cancellationToken);

            await publisher.PublishAsync(new Message { Property = 1, TenantId = "1" }, cancellationToken);
            await publisher.PublishAsync(new Message { Property = 2, TenantId = "2" }, cancellationToken);
            await publisher.PublishAsync(new Message { Property = 3, TenantId = "3" }, cancellationToken);
            await publisher.PublishAsync(new Message { Property = 4, TenantId = "4" }, cancellationToken);

            //--------------------------------------------


            // Subcribe using ServiceBusSubscriber
            //--------------------------------------------
            var serviceBusSubcriberOptions = new ServiceBusSubscriberOptions
            {
                ConnectionString = "Endpoint=sb://invoicebuso27h3yhrdfv74.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=qF7r1IOVaaiBQW9zMtD2zxB9Dkvt0+HqFVHU9HxjI+8=",
                TopicName = "InvoiceManagement.Sample",
                SubscriberName = "Sample.Subscriber",
                MessageLockDuration = TimeSpan.FromSeconds(20)
            };

            var telemetryClient = new EmptyTelemetryClient();
            var exceptionHandler = new DefaultExceptionHandler(telemetryClient);
            var applicationContext = new BackgroundServiceContext();
            var messageHandlers = new List<IMessageHandler> { new MessageHandler(telemetryClient, applicationContext) };
            var subcriber = new ServiceBusSubscriberFactory().Create(messageHandlers, exceptionHandler, serviceBusSubcriberOptions);

            await subcriber.StartAsync(cancellationToken);

            //--------------------------------------------

            // Give some time for message handlers before blocking the Console (for test purposes only)
            await Task.Delay(5000);

            // Release Resources
            //--------------------------------------------

            Console.ReadKey();
            await publisher.StopAsync(cancellationToken);
            await subcriber.StopAsync(cancellationToken);
        }

        private static readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
