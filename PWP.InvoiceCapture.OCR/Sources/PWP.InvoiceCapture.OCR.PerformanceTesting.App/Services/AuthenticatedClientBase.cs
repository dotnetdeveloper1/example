using Newtonsoft.Json;
using PWP.InvoiceCapture.Core.Communication;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services
{
    internal class AuthenticatedClientBase : ApiClientBase
    {
        public AuthenticatedClientBase(AuthenticationOptions authenticationOptions) : base(authenticationOptions)
        {
            this.authenticationOptions = authenticationOptions;
        }

        protected new async Task ExecuteWithRetryAsync(Func<Task> action) 
        {
            await AuthorizeAsync();
            await base.ExecuteWithRetryAsync(action);
        }

        protected new async Task<TResult> ExecuteWithRetryAsync<TResult>(Func<Task<TResult>> action) 
        {
            await AuthorizeAsync();
            
            return await base.ExecuteWithRetryAsync(action);
        }

        protected new async Task<HttpResponseMessage> ExecuteWithRetryAsync(Func<Task<HttpResponseMessage>> action) 
        {
            await AuthorizeAsync();

            return await base.ExecuteWithRetryAsync(action);
        }

        protected async Task AuthorizeAsync() 
        {
            var token = await GetTokenAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> GetTokenAsync()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("scope", "InvoiceManagement"),
                new KeyValuePair<string, string>("client_id", authenticationOptions.ClientId),
                new KeyValuePair<string, string>("client_secret", authenticationOptions.ClientSecret),
                new KeyValuePair<string, string>("username", authenticationOptions.Username),
                new KeyValuePair<string, string>("password", authenticationOptions.Password),
                new KeyValuePair<string, string>("tenantId", authenticationOptions.TenantId.ToString()),
            });

            var response = await base.ExecuteWithRetryAsync(() =>
               client.PostAsync("connect/token", content, cancellationToken));

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Token>(stringResponse);
            
            return token.access_token;
        }

        private readonly AuthenticationOptions authenticationOptions;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
