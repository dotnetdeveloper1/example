using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IManagementClient
    {
        Task CreateOrUpdateTopicAsync(string topicName, CancellationToken cancellationToken);
        Task CreateOrUpdateSubsciptionAsync(string topicName, string subscriptionName, TimeSpan lockDuration, CancellationToken cancellationToken);
        Task CreateRulesAsync(string topicName, string subscriptionName, IEnumerable<RuleDescription> rules, CancellationToken cancellationToken);
        Task DeleteRulesAsync(string topicName, string subscriptionName, IEnumerable<string> ruleNames, CancellationToken cancellationToken);
        Task<List<RuleDescription>> GetRulesAsync(string topicName, string subscriptionName, CancellationToken cancellationToken);
    }
}
