using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PWP.InvoiceCapture.Core.Communication;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Models;
using PWP.InvoiceCapture.Document.API.Client.Options;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Document.API.Client
{
    public class DocumentApiClient : ApiClientBase, IDocumentApiClient
    {
        public DocumentApiClient(IOptions<DocumentApiClientOptions> options) : base(options.Value)
        { }

        public async Task<Stream> GetDocumentStreamAsync(string fileId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));

            return await ExecuteWithRetryAsync(() => 
                client.GetStreamAsync($"documents/{fileId}"));
        }

        public async Task<ApiResponse<UploadDocumentResponse>> UploadDocumentAsync(Stream content, string fileName, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(content, nameof(content));
            Guard.IsNotNullOrWhiteSpace(fileName, nameof(fileName));

            var multipartContent = new MultipartFormDataContent();
            var streamContent = new StreamContent(content);

            multipartContent.Add(streamContent, "file", fileName);

            var response = await ExecuteWithRetryAsync(() => 
                client.PostAsync("documents", multipartContent));
            
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<ApiResponse<UploadDocumentResponse>>(stringResponse);
        }

        public async Task<ApiResponse<string>> GetTemporaryLinkAsync(string fileId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));

            var response = await ExecuteWithRetryAsync(() =>
               client.GetAsync($"documents/{fileId}/link"));

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse<string>>(stringResponse);
        }
    }
}
