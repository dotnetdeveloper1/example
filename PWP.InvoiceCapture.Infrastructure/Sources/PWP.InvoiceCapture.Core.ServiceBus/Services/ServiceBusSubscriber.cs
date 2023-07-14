using Microsoft.Azure.ServiceBus;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Services
{
    public class ServiceBusSubscriber : IServiceBusSubscriber
    {
        public ServiceBusSubscriber(IManagementClientFactory managementClientFactory, ISubscriptionClientFactory subscriptionClientFactory, IMessageTypeProvider messageTypeProvider, IExceptionHandler exceptionHandler, IEnumerable<IMessageHandler> messageHandlers, IMessageSerializer messageSerializer, ServiceBusSubscriberOptions options) 
        {
            Guard.IsNotNull(managementClientFactory, nameof(managementClientFactory));
            Guard.IsNotNull(subscriptionClientFactory, nameof(subscriptionClientFactory));
            Guard.IsNotNull(messageTypeProvider, nameof(messageTypeProvider));
            Guard.IsNotNull(exceptionHandler, nameof(exceptionHandler));
            Guard.IsNotNull(messageSerializer, nameof(messageSerializer));
            Guard.IsNotNullOrEmpty(messageHandlers, nameof(messageHandlers));
            ValidateSubscriberOptions(options);

            this.managementClientFactory = managementClientFactory;
            this.subscriptionClientFactory = subscriptionClientFactory;
            this.exceptionHandler = exceptionHandler;
            this.messageSerializer = messageSerializer;
            this.messageHandlers = messageHandlers.ToDictionary(handler => messageTypeProvider.Get(handler.MessageType));
            this.options = options;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var managementClient = await managementClientFactory.CreateAsync(options.ConnectionString, cancellationToken);

            if (managementClient == null)
            {
                throw new InvalidOperationException("Cannot start ServiceBusSubscriber because ManagementClientFactory returned null.");
            }

            await managementClient.CreateOrUpdateTopicAsync(options.TopicName, cancellationToken);
            await managementClient.CreateOrUpdateSubsciptionAsync(options.TopicName, options.SubscriberName, options.MessageLockDuration, cancellationToken);
            await RegisterFiltersAsync(managementClient, cancellationToken);

            subscriptionClient = subscriptionClientFactory.Create(options.ConnectionString, options.TopicName, options.SubscriberName, receiveMode, retryPolicy);

            RegisterMessageHandler();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (subscriptionClient != null)
            {
                await subscriptionClient.CloseAsync();
            }
        }

        protected async Task ProcessMessagesAsync(Message message, CancellationToken cancellationToken)
        {
            var lockToken = GetLockToken(message);

            if (cancellationToken.IsCancellationRequested)
            {
                await subscriptionClient.AbandonAsync(lockToken);

                return;
            }

            var messageType = message.Label;

            if (!messageHandlers.ContainsKey(messageType))
            {
                throw new InvalidOperationException($"Message cannot be handled. There is no message handler registered for the received message type '{messageType}'.");
            }

            var messageHandler = messageHandlers[messageType];

            var brokeredMessage = new BrokeredMessage
            {
                Id = message.MessageId,
                CorrelationId = message.CorrelationId,
                MessageType = message.Label,
                InnerMessage = messageSerializer.Deserialize(message.Body, messageHandler.MessageType)
            };

            await messageHandler.HandleAsync(brokeredMessage, cancellationToken);
            await subscriptionClient.CompleteAsync(lockToken);
        }

        private string GetLockToken(Message message) 
        { 
            return message.SystemProperties.IsLockTokenSet
                ? message.SystemProperties.LockToken
                : null; 
        }

        private async Task RegisterFiltersAsync(IManagementClient managementClient, CancellationToken cancellationToken) 
        {
            var messageTypes = messageHandlers.Keys.ToList();
            var existingRules = await managementClient.GetRulesAsync(options.TopicName, options.SubscriberName, cancellationToken);
            var existingMessageTypes = existingRules.Select(rule => rule.Name).ToList();

            var rulesToDelete = existingMessageTypes.Where(messageType => !messageTypes.Contains(messageType));
            
            var rulesToCreate = messageTypes
                .Where(messageType => !existingMessageTypes.Contains(messageType))
                .Select(messageType => 
                    new RuleDescription(messageType, 
                        new CorrelationFilter { Label = messageType }));

            if (rulesToDelete.Any())
            {
                await managementClient.DeleteRulesAsync(options.TopicName, options.SubscriberName, rulesToDelete, cancellationToken);
            }

            if (rulesToCreate.Any())
            {
                await managementClient.CreateRulesAsync(options.TopicName, options.SubscriberName, rulesToCreate, cancellationToken);
            }
        }

        private void RegisterMessageHandler() 
        {
            var messageHandlerOptions = new MessageHandlerOptions(exceptionHandler.HandleAsync)
            {
                AutoComplete = false,
                MaxConcurrentCalls = options.MaxConcurrentCalls
            };

            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private void ValidateSubscriberOptions(ServiceBusSubscriberOptions options)
        {
            Guard.IsNotNull(options, nameof(options));
            Guard.IsNotNullOrWhiteSpace(options.ConnectionString, nameof(options.ConnectionString));
            Guard.IsNotNullOrWhiteSpace(options.TopicName, nameof(options.TopicName));
            Guard.IsNotNullOrWhiteSpace(options.SubscriberName, nameof(options.SubscriberName));
            Guard.IsNotZeroOrNegative(options.MaxConcurrentCalls, nameof(options.MaxConcurrentCalls));
        }

        private ISubscriptionClient subscriptionClient;
        private readonly RetryPolicy retryPolicy = RetryPolicy.Default;
        private readonly ReceiveMode receiveMode = ReceiveMode.PeekLock;
        private readonly IManagementClientFactory managementClientFactory;
        private readonly ISubscriptionClientFactory subscriptionClientFactory;
        private readonly IExceptionHandler exceptionHandler;
        private readonly IMessageSerializer messageSerializer;
        private readonly ServiceBusSubscriberOptions options;
        private readonly Dictionary<string, IMessageHandler> messageHandlers;
    }
}
