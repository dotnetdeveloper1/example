using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Management
{
    public class ManagementClientAdapter : IManagementClient
    {
        public ManagementClientAdapter(ManagementClient client) 
        {
            Guard.IsNotNull(client, nameof(client));

            this.client = client;
        }

        public async Task CreateOrUpdateTopicAsync(string topicName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(topicName, nameof(topicName));

            if (!await client.TopicExistsAsync(topicName, cancellationToken))
            {
                await TryCreateServiceBusResourceAsync(() =>
                    client.CreateTopicAsync(topicName, cancellationToken));
            }
        }

        public async Task CreateOrUpdateSubsciptionAsync(string topicName, string subscriptionName, TimeSpan lockDuration, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(topicName, nameof(topicName));
            Guard.IsNotNullOrWhiteSpace(subscriptionName, nameof(subscriptionName));

            var subscriptionDescription = new SubscriptionDescription(topicName, subscriptionName)
            {
                LockDuration = lockDuration
            };

            if (!await client.SubscriptionExistsAsync(topicName, subscriptionName, cancellationToken))
            {
                await TryCreateServiceBusResourceAsync(() =>
                    client.CreateSubscriptionAsync(subscriptionDescription, cancellationToken));
            }
            else
            {
                await client.UpdateSubscriptionAsync(subscriptionDescription, cancellationToken);
            }
        }

        public async Task CreateRulesAsync(string topicName, string subscriptionName, IEnumerable<RuleDescription> rules, CancellationToken cancellationToken)
        {
            var tasks = rules.Select(rule => 
                TryCreateServiceBusResourceAsync(() => 
                    client.CreateRuleAsync(topicName, subscriptionName, rule, cancellationToken)));

            await Task.WhenAll(tasks);
        }

        public async Task DeleteRulesAsync(string topicName, string subscriptionName, IEnumerable<string> ruleNames, CancellationToken cancellationToken)
        {
            var tasks = ruleNames.Select(ruleName =>
                TryDeleteServiceBusResourceAsync(() =>
                    client.DeleteRuleAsync(topicName, subscriptionName, ruleName, cancellationToken)));

            await Task.WhenAll(tasks);
        }

        public async Task<List<RuleDescription>> GetRulesAsync(string topicName, string subscriptionName, CancellationToken cancellationToken)
        {
            IList<RuleDescription> operationResult;
            var rules = new List<RuleDescription>();
            var take = 100;
            var skip = 0;

            do
            {
                operationResult = await client.GetRulesAsync(topicName, subscriptionName, take, skip, cancellationToken);
                skip += take;

                rules.AddRange(operationResult);
            }
            while (operationResult.Count > 0);

            return rules;
        }

        private async Task TryDeleteServiceBusResourceAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (MessagingEntityNotFoundException)
            {
                // It's possible the resource was already deleted in parrallel by another instance/service. Do nothing in this case.
            }
        }

        private async Task TryCreateServiceBusResourceAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                // It's possible the resource is already created in parrallel by another instance/service. Do nothing in this case.
            }
        }

        private readonly ManagementClient client;
    }
}
