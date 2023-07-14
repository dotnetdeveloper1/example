using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Options;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services
{
    internal class DocumentAggregationClient : AuthenticatedClientBase,  IDocumentAggregationClient
    {
        public DocumentAggregationClient(IOptions<AuthenticationOptions> options) : base(options.Value)
        { }

        public async Task<ApiResponse<UploadDocumentResponse>> UploadFileAsync(Stream content, string fileName, CancellationToken cancellationToken)
        {
            var multipartContent = new MultipartFormDataContent();
            var streamContent = new StreamContent(content);
            
            multipartContent.Add(streamContent, "file", fileName);
            
            var response = await ExecuteWithRetryAsync(() =>
                client.PostAsync("invoiceDocuments", multipartContent, cancellationToken));

            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<ApiResponse<UploadDocumentResponse>>(stringResponse);
        }
    }
}
