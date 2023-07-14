using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories
{
    internal class WebhookRepository : IWebhookRepository
    {
        public WebhookRepository(IDatabaseContextFactory contextFactory) 
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<Webhook> GetAsync(int id, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));

            using (var context = contextFactory.Create())
            {
                return await context.Webhooks.FirstOrDefaultAsync(webhook => webhook.Id == id);
            }
        }

        public async Task<bool> AnyAsync(TriggerType triggerType, string url, CancellationToken cancellationToken)
        {
            Guard.IsEnumDefined(triggerType, nameof(triggerType));
            Guard.IsNotNullOrWhiteSpace(url, nameof(url));

            using (var context = contextFactory.Create())
            {
                return await context.Webhooks.AnyAsync(webhook => webhook.TriggerType == triggerType && webhook.Url == url, cancellationToken);
            }
        }

        public async Task<bool> AnyAsync(TriggerType triggerType, string url, int excludedId, CancellationToken cancellationToken)
        {
            Guard.IsEnumDefined(triggerType, nameof(triggerType));
            Guard.IsNotNullOrWhiteSpace(url, nameof(url));

            using (var context = contextFactory.Create())
            {
                return await context.Webhooks.AnyAsync(webhook => 
                    webhook.TriggerType == triggerType
                    && webhook.Url == url
                    && webhook.Id != excludedId, 
                    cancellationToken);
            }
        }

        public async Task<List<Webhook>> GetListAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.Webhooks.ToListAsync(cancellationToken);
            }
        }

        public async Task CreateAsync(Webhook webhook, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(webhook, nameof(webhook));

            var currentDate = DateTime.UtcNow;

            webhook.CreatedDate = currentDate;
            webhook.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Webhooks.Add(webhook);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(int webhookId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(webhookId, nameof(webhookId));

            using (var context = contextFactory.Create())
            {
                context.Webhooks.RemoveRange(
                    context.Webhooks.Where(webhook => webhook.Id == webhookId));

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateAsync(int webhookId, Webhook webhook, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(webhookId, nameof(webhookId));
            Guard.IsNotNull(webhook, nameof(webhook));

            webhook.ModifiedDate = DateTime.UtcNow;
            webhook.Id = webhookId;

            using (var context = contextFactory.Create())
            {
                var entityEntry = context.Entry(webhook);

                entityEntry.State = EntityState.Modified;
                entityEntry.Property(entity => entity.CreatedDate).IsModified = false;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private readonly IDatabaseContextFactory contextFactory;
    }
}
