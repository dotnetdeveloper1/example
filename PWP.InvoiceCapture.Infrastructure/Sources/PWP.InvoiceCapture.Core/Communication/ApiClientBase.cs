using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.Communication
{
    public abstract class ApiClientBase
    {
        public ApiClientBase(ApiClientOptions options)
        {
            Guard.IsNotNull(options, nameof(options));
            Guard.IsNotNullOrWhiteSpace(options.BaseAddress, nameof(options.BaseAddress));
            Guard.IsNotZeroOrNegative(options.RetryAttemptCount, nameof(options.RetryAttemptCount));
            Guard.IsNotZeroOrNegative(options.TimeoutInSeconds, nameof(options.TimeoutInSeconds));

            this.options = options;

            retryPolicy = CreateRetryPolicy();
            httpRetryPolicy = CreateHttpRetryPolicy();

            SetupHttpClient();
        }

        protected Task ExecuteWithRetryAsync(Func<Task> action) => retryPolicy.ExecuteAsync(action);
        
        protected Task<TResult> ExecuteWithRetryAsync<TResult>(Func<Task<TResult>> action) => retryPolicy.ExecuteAsync(action);
        
        protected Task<HttpResponseMessage> ExecuteWithRetryAsync(Func<Task<HttpResponseMessage>> action) => httpRetryPolicy.ExecuteAsync(action);

        protected virtual void SetupHttpClient()
        {
            client.BaseAddress = new Uri(options.BaseAddress);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutInSeconds);
        }

        protected virtual AsyncRetryPolicy<HttpResponseMessage> CreateHttpRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(options.RetryAttemptCount, GetWaitInterval);
        }

        protected virtual AsyncRetryPolicy CreateRetryPolicy()
        {
            return Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(options.RetryAttemptCount, GetWaitInterval);
        }

        protected virtual TimeSpan GetWaitInterval(int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));

        protected readonly HttpClient client = new HttpClient();
        protected readonly AsyncRetryPolicy<HttpResponseMessage> httpRetryPolicy;
        protected readonly AsyncRetryPolicy retryPolicy;
        protected readonly ApiClientOptions options;
    }
}
