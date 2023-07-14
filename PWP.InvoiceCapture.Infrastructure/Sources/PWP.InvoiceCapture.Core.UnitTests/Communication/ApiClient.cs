using Polly.Retry;
using PWP.InvoiceCapture.Core.Communication;
using PWP.InvoiceCapture.Core.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace PWP.InvoiceCapture.Core.UnitTests.Communication
{
    [ExcludeFromCodeCoverage]
    internal class ApiClient : ApiClientBase
    {
        public ApiClient(ApiClientOptions options) : base(options)
        { }

        public AsyncRetryPolicy<HttpResponseMessage> HttpRetryPolicy => httpRetryPolicy;

        public AsyncRetryPolicy RetryPolicy => retryPolicy;

        public HttpClient HttpClient => client;

        public new TimeSpan GetWaitInterval(int retryAttempt) => base.GetWaitInterval(retryAttempt);
    }
}
