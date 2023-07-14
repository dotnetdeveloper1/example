using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class WebhookService : IWebhookService
    {
        public WebhookService(IWebhookRepository hooksRegistrationRepository) 
        {
            Guard.IsNotNull(hooksRegistrationRepository, nameof(hooksRegistrationRepository));

            this.hooksRegistrationRepository = hooksRegistrationRepository;
        }

        public async Task<Webhook> GetAsync(int id, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));

            return await hooksRegistrationRepository.GetAsync(id, cancellationToken);
        }

        public async Task<List<Webhook>> GetListAsync(CancellationToken cancellationToken)
        {
            return await hooksRegistrationRepository.GetListAsync(cancellationToken);
        }

        public async Task<OperationResult> CreateAsync(Webhook webhook, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(webhook, nameof(webhook));

            var urlParsingResult = ParseUrl(webhook.Url);

            if (!urlParsingResult.IsSuccessful)
            {
                return urlParsingResult;
            }

            if (!TriggerTypeIsDefined(webhook.TriggerType))
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Webhook contains unknown trigger type."
                };
            }

            var alreadyHasSameHook = await hooksRegistrationRepository.AnyAsync(webhook.TriggerType, webhook.Url, cancellationToken);
            if (alreadyHasSameHook)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Web hook with trigger type: {webhook.TriggerType} and url: {webhook.Url} already exists."
                };
            }

            await hooksRegistrationRepository.CreateAsync(webhook, cancellationToken);

            return new OperationResult { Status = OperationResultStatus.Success };
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));

            await hooksRegistrationRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<OperationResult> UpdateAsync(int id, Webhook webhook, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));
            Guard.IsNotNull(webhook, nameof(webhook));

            var urlParsingResult = ParseUrl(webhook.Url);

            if (!urlParsingResult.IsSuccessful)
            {
                return urlParsingResult;
            }

            if (!TriggerTypeIsDefined(webhook.TriggerType))
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Webhook contains unknown trigger type."
                };
            }

            var alreadyHasSameHook = await hooksRegistrationRepository.AnyAsync(webhook.TriggerType, webhook.Url, id, cancellationToken);
            if (alreadyHasSameHook)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Web hook with trigger type: {webhook.TriggerType} and url: {webhook.Url} already exists."
                };
            }

            await hooksRegistrationRepository.UpdateAsync(id, webhook, cancellationToken);

            return new OperationResult { Status = OperationResultStatus.Success };
        }

        public Dictionary<string, int> GetTriggerTypes()
        {
            return EnumExtensions.GetKeyValues<TriggerType, int>();
        }

        private bool TriggerTypeIsDefined(TriggerType triggerType)
        {
            return Enum.IsDefined(typeof(TriggerType), triggerType);
        }

        private OperationResult ParseUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Webhook contains empty url."
                };
            }

            try
            {
                new Uri(url);
            }
            catch
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Webhook contains wrong url."
                };
            }

            return new OperationResult
            {
                Status = OperationResultStatus.Success
            };
        }
        
        private readonly IWebhookRepository hooksRegistrationRepository;
    }
}
