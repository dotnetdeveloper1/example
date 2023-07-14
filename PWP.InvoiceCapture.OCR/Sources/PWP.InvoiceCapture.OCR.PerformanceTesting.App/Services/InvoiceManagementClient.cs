using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Options;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Invoice = PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models.Invoice;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services
{
    internal class InvoiceManagementClient : AuthenticatedClientBase,  IInvoiceManagementClient
    {
        public InvoiceManagementClient(IOptions<AuthenticationOptions> options) : base(options.Value)
        { }

        public async Task<List<Invoice>> GetListAsync(CancellationToken cancellationToken)
        {
            var response = await ExecuteWithRetryAsync(() =>
                client.GetAsync($"invoices", cancellationToken));

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var invoices = JsonConvert.DeserializeObject<ApiResponse<List<Invoice>>>(stringResponse);

            return invoices.Data;
        }

        public async Task<string> GetDocumentFileLinkAsync(int invoiceId, CancellationToken cancellationToken)
        {
            var response = await ExecuteWithRetryAsync(() =>
                client.GetAsync($"invoices/{invoiceId}/document", cancellationToken));

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var link = JsonConvert.DeserializeObject<ApiResponse<string>>(stringResponse);

            return link.Data;
        }

        public async Task<Invoice> GetInvoiceByDocumentIdAsync(string documentId, CancellationToken cancellationToken)
        {
            var response = await ExecuteWithRetryAsync(() => 
                client.GetAsync($"invoices/documents/{documentId}", cancellationToken));
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var invoice = JsonConvert.DeserializeObject<ApiResponse<Invoice>>(stringResponse);
            
            return invoice.Data;
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int invoiceId, CancellationToken cancellationtoken)
        {
            var response = await ExecuteWithRetryAsync(() =>
                client.GetAsync($"invoices/{invoiceId}", cancellationtoken));
            
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var invoice = JsonConvert.DeserializeObject<ApiResponse<Invoice>>(stringResponse);
            
            return invoice.Data;
        }

        public async Task<InvoiceProcessingResult> GetLatestProcessingResultByInvoiceIdAsync(int invoiceId, CancellationToken cancellationtoken)
        {
            var response = await ExecuteWithRetryAsync(() =>
                client.GetAsync($"processingresults/invoices/{invoiceId}/latest", cancellationtoken));
            
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var invoiceProcessingResult = JsonConvert.DeserializeObject<ApiResponse<InvoiceProcessingResult>>(stringResponse);

            return invoiceProcessingResult.Data;
        }

        public async Task CompleteAsync(int processingResultId, DataAnnotation dataAnnotation, CancellationToken cancellationToken)
        {
            var updatedDataAnnotation = new UpdatedDataAnnotation
            {
                DataAnnotation = dataAnnotation
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedDataAnnotation), Encoding.UTF8, "application/json");
            
            var response = await ExecuteWithRetryAsync(() =>
                client.PutAsync($"processingresults/{processingResultId}/complete", content, cancellationToken));
            
            response.EnsureSuccessStatusCode();
        }
    }
}
