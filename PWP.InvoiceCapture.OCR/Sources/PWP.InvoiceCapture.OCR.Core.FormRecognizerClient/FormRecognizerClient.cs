using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using PWP.InvoiceCapture.Core.Communication;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FormRecognizerClientOptions = PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Options.FormRecognizerClientOptions;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client
{
    internal class FormRecognizerClient : ApiClientBase, IFormRecognizerClient
    {
        public FormRecognizerClient(FormRecognizerClientOptions options, ISerializationService serializationService, int formRecognizerId) : base(options)
        {
            Guard.IsNotNull(options, nameof(options));
            Guard.IsNotNull(serializationService, nameof(serializationService));
            Guard.IsNotNullOrWhiteSpace(options.ApiKey, nameof(options.ApiKey));
            Guard.IsNotZeroOrNegative(options.RetryAttemptCount, nameof(options.RetryAttemptCount));
            Guard.IsNotZeroOrNegative(formRecognizerId, nameof(formRecognizerId));
            
            apiKey = options.ApiKey;
            baseAddress = options.BaseAddress;
            retryCount = options.RetryAttemptCount;
            
            this.serializationService = serializationService;
            FormRecognizerId = formRecognizerId;
        }

        public int FormRecognizerId { get; }

        public async Task<FormRecognizerResponse> RunLayoutAnalysisAsync(string sasUri, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(sasUri, nameof(sasUri));

            var apiResponse = await InitiateLayoutAnalysisAsync(sasUri, cancellationToken);
            var resultLocation = apiResponse.ResourceLocation;
            
            var response = await GetAnalysisResponseWithRetryAsync(resultLocation, cancellationToken);
            response.InvoiceFields = await GetInvoiceFieldsAsync(sasUri, cancellationToken);

            return response;
        }

        public async Task<FormRecognizerResponse> RunFormAnalysisAsync(string sasUri, string modelId, int formRecognizerId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(sasUri, nameof(sasUri));
            Guard.IsNotNullOrEmpty(modelId, nameof(modelId));

            var apiResponse = await InitiateFormAnalysisAsync(sasUri, modelId, cancellationToken);
            var resultLocation = apiResponse.ResourceLocation;
            
            return await GetAnalysisResponseWithRetryAsync(resultLocation, cancellationToken);
        }

        public async Task<AwaitModelReadinessResponse> AwaitModelReadinessAsync(string modelId, int formRecognizerId, CancellationToken cancellationToken)
        {
            var response = await GetModelDetailsAsync(modelId, formRecognizerId, cancellationToken);
            var succeeded = response.ModelInfo.Status.Equals(trainingSuccessIdentifier);
            var currentCount = 1;
            
            while (!succeeded && currentCount <= retryCount && (response.TrainResult?.Errors == null || response.TrainResult.Errors.Count == 0))
            {
                await CreateRetryDelay(currentCount);
                response = await GetModelDetailsAsync(modelId, formRecognizerId, cancellationToken);
                succeeded = response.ModelInfo.Status.Equals(trainingSuccessIdentifier);
                currentCount++;
            }

            return new AwaitModelReadinessResponse { IsModelReady = succeeded, ModelDetails = response, ResponseDocument = response.ResponseDocument };
        }

        public async Task<TrainModelResponse> TrainModelAsync(string sasUri, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(sasUri, nameof(sasUri));

            var apiResponse = await InitiateTrainModelAsync(sasUri, cancellationToken);
            var resultLocation = apiResponse.ResourceLocation;

            return await GetTrainModelWithRetryAsync(resultLocation, cancellationToken);
        }

        public async Task DeleteModelAsync(string modelId, int formRecognizerId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(modelId, nameof(modelId));
            
            var uri = $"{modelsEndpoint}/{modelId}";
            var request = GetRequestMessage(HttpMethod.Delete, uri);

            var response = await ExecuteWithRetryAsync(() => client.SendAsync(request, cancellationToken));
            response.EnsureSuccessStatusCode();
        }

        public async Task<TrainModelResponse> GetModelDetailsAsync(string modelId, int formRecognizerId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(modelId, nameof(modelId));

            var uri = $"{modelsEndpoint}/{modelId}";

            return await GetTrainModelResponseAsync(uri, cancellationToken);
        }

        public async Task<ListModelResponse> GetListModelResponseAsync(int formRecognizerId, CancellationToken cancellationToken)
        {
            var location = $"{options.BaseAddress}/{modelsEndpoint}?op=full";
            var apiResponse = await GetResultAsync(location, cancellationToken);
            var response = serializationService.Deserialize<ListModelResponse>(apiResponse.ResponseDocument);

            response.ResponseDocument = apiResponse.ResponseDocument;

            return response;
        }

        private async Task<Dictionary<string, FormField>> GetInvoiceFieldsAsync(string sasUri, CancellationToken cancellationToken) 
        {
            var credentials = new AzureKeyCredential(apiKey);
            var baseAddressUri = new Uri(baseAddress);
            var client = new Azure.AI.FormRecognizer.FormRecognizerClient(baseAddressUri, credentials);

            var sasAddressUri = new Uri(sasUri);
            var recognitionOptions = new RecognizeInvoicesOptions { IncludeFieldElements = false };

            var result = await client
                .StartRecognizeInvoicesFromUriAsync(sasAddressUri, recognitionOptions, cancellationToken)
                .WaitForCompletionAsync(cancellationToken);

            return result?.Value?.FirstOrDefault()?.Fields?.ToDictionary(item => item.Key, item => item.Value);
        }

        private async Task<FormRecognizerResponse> GetAnalysisResponseWithRetryAsync(string resultLocation, CancellationToken cancellationToken)
        {
            var response = await GetAnalysisResponseAsync(resultLocation, cancellationToken);
            var currentCount = 1;

            while (!response.Status.Equals(analysisSuccessIdentifier) && !response.Status.Equals(analysisFailedIdentifier) && currentCount <= retryCount)
            {
                await CreateRetryDelay(currentCount);
                response = await GetAnalysisResponseAsync(resultLocation, cancellationToken);
                currentCount++;
            }

            return response;
        }

        private async Task<TrainModelResponse> GetTrainModelWithRetryAsync(string resultLocation, CancellationToken cancellationToken)
        {
            var response = await GetTrainModelResponseAsync(resultLocation, cancellationToken);
            var currentCount = 1;

            while (!response.ModelInfo.Status.Equals(trainingSuccessIdentifier) && currentCount <= retryCount && (response.TrainResult?.Errors == null || response.TrainResult.Errors.Count == 0))
            {
                await CreateRetryDelay(currentCount);
                response = await GetTrainModelResponseAsync(resultLocation, cancellationToken);
                currentCount++;
            }

            return response;
        }

        private async Task<OCRProviderResponse> GetResultAsync(string location, CancellationToken cancellationToken)
        {
            var response = await GetApiResultAsync(location, cancellationToken);
            var apiResponse = new OCRProviderResponse
            {
                ResponseDocument = await response.Content.ReadAsStringAsync()
            };

            return apiResponse;
        }

        private async Task<FormRecognizerResponse> GetAnalysisResponseAsync(string location, CancellationToken cancellationToken)
        {
            var apiResponse = await GetResultAsync(location, cancellationToken);
            var response = serializationService.Deserialize<FormRecognizerResponse>(apiResponse.ResponseDocument);

            response.ResponseDocument = apiResponse.ResponseDocument;
            
            return response;
        }

        private async Task<TrainModelResponse> GetTrainModelResponseAsync(string location, CancellationToken cancellationToken)
        {
            var apiResponse = await GetResultAsync(location, cancellationToken);
            var response = serializationService.Deserialize<TrainModelResponse>(apiResponse.ResponseDocument);

            response.ResponseDocument = apiResponse.ResponseDocument;
                
            return response;
        }

        private HttpRequestMessage GetRequestMessage(HttpMethod method, string location)
        {
            var request = new HttpRequestMessage(method, location);
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
            
            return request;
        }

        private async Task<InitiateAnalysisResult> InitiateAnalysisAsync(HttpMethod method, string uri, object payload, string locationHeaderIdentifier, CancellationToken cancellationToken)
        {
            var request = GetRequestMessage(method, uri);
            request.Content = new StringContent(serializationService.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await ExecuteWithRetryAsync(() => client.SendAsync(request, cancellationToken));
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(responseText);
            }

            var values = response.Headers.GetValues(locationHeaderIdentifier);
            
            return new InitiateAnalysisResult
            {
                ResourceLocation = values.First(),
                ResponseDocument = responseText
            };
        }

        private async Task<InitiateAnalysisResult> InitiateLayoutAnalysisAsync(string sasUri,CancellationToken cancellationToken)
        {
            var uri = $"{options.BaseAddress}/{analyzeLayoutEndpoint}";
            var payload = new { url = sasUri };

            return await InitiateAnalysisAsync(HttpMethod.Post, uri, payload, operationLocationHeader, cancellationToken);
        }

        private async Task<InitiateAnalysisResult> InitiateFormAnalysisAsync(string sasUri, string modelId, CancellationToken cancellationToken)
        {
            var uri = $"{options.BaseAddress}/{analyzeFormEndpoint}/{modelId}/analyze?includeTextDetails=true";
            var payload = new { source = sasUri };

            return await InitiateAnalysisAsync(HttpMethod.Post, uri, payload, operationLocationHeader, cancellationToken);
        }

        private async Task<InitiateAnalysisResult> InitiateTrainModelAsync(string sasUri, CancellationToken cancellationToken)
        {
            var uri = $"{options.BaseAddress}/{modelsEndpoint}";
            var sourceFilterObject = new { includeSubFolders = false, prefix = "" };
            var payload = new { source = sasUri,  sourceFilter = sourceFilterObject, useLabelFile = true};

            return await InitiateAnalysisAsync(HttpMethod.Post, uri, payload, locationHeader, cancellationToken);
        }

        private async Task<HttpResponseMessage> GetApiResultAsync(string resultLocation, CancellationToken cancellationToken)
        {
            var request = GetRequestMessage(HttpMethod.Get, resultLocation);
            var response = await ExecuteWithRetryAsync(() => client.SendAsync(request, cancellationToken));

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception(content);
            }

            return response;
        }

        private static Task CreateRetryDelay(int currentRetryAttempt)
        {
            var delay = (int)TimeSpan.FromSeconds(currentRetryAttempt * retryOffset).TotalMilliseconds;

            return Task.Delay(delay);
        }

        private readonly string apiKey;
        private readonly string baseAddress;
        private readonly static int retryOffset = 5;
        private readonly int retryCount;
        private readonly ISerializationService serializationService;
        private const string analyzeLayoutEndpoint = "formrecognizer/v2.0/layout/analyze";
        private const string analyzeFormEndpoint = "formrecognizer/v2.0/custom/models";
        private const string modelsEndpoint = "formrecognizer/v2.0/custom/models";
        private const string operationLocationHeader = "Operation-Location";
        private const string locationHeader = "Location";
        private const string analysisSuccessIdentifier = "succeeded";
        private const string trainingSuccessIdentifier = "ready";
        private const string analysisFailedIdentifier = "failed";
    }
}
