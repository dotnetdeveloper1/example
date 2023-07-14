using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client
{
    internal class FormRecognizerClientPoolDecorator : IFormRecognizerClient
    {
        public FormRecognizerClientPoolDecorator(IFormRecognizerClientService formRecognizerClientService)
        {
            Guard.IsNotNull(formRecognizerClientService, nameof(formRecognizerClientService));

            this.formRecognizerClientService = formRecognizerClientService;
        }

        public int FormRecognizerId => target?.FormRecognizerId ?? 0;

        public async Task<AwaitModelReadinessResponse> AwaitModelReadinessAsync(string modelId, int formRecognizerId, CancellationToken cancellationToken)
        {
            return await ExecuteOnSpecificResourceAsync((client) => 
                client.AwaitModelReadinessAsync(modelId, formRecognizerId, cancellationToken), 
                formRecognizerId, 
                cancellationToken);
        }

        public async Task DeleteModelAsync(string modelId, int formRecognizerId, CancellationToken cancellationToken)
        {
            await ExecuteOnSpecificResourceAsync((client) => 
                client.DeleteModelAsync(modelId, formRecognizerId, cancellationToken), 
                formRecognizerId, 
                cancellationToken);
        }

        public async Task<ListModelResponse> GetListModelResponseAsync(int formRecognizerId, CancellationToken cancellationToken)
        {
            return await ExecuteOnSpecificResourceAsync((client) =>
                client.GetListModelResponseAsync(formRecognizerId, cancellationToken),
                formRecognizerId,
                cancellationToken);
        }

        public async Task<TrainModelResponse> GetModelDetailsAsync(string modelId, int formRecognizerId, CancellationToken cancellationToken)
        {
            return await ExecuteOnSpecificResourceAsync((client) =>
                client.GetModelDetailsAsync(modelId, formRecognizerId, cancellationToken),
                formRecognizerId,
                cancellationToken);
        }

        public async Task<FormRecognizerResponse> RunFormAnalysisAsync(string sasUri, string modelId, int formRecognizerId, CancellationToken cancellationToken)
        {
            return await ExecuteOnSpecificResourceAsync((client) =>
                client.RunFormAnalysisAsync(sasUri, modelId, formRecognizerId, cancellationToken),
                formRecognizerId,
                cancellationToken);
        }

        public async Task<FormRecognizerResponse> RunLayoutAnalysisAsync(string sasUri, CancellationToken cancellationToken)
        {
            return await ExecuteOnAnyResourceAsync((client) =>
                client.RunLayoutAnalysisAsync(sasUri, cancellationToken),
                cancellationToken);
        }

        public async Task<TrainModelResponse> TrainModelAsync(string sasUri, CancellationToken cancellationToken)
        {
            return await ExecuteOnAnyResourceAsync((client) =>
                client.TrainModelAsync(sasUri, cancellationToken),
                cancellationToken);
        }

        private async Task ExecuteOnSpecificResourceAsync(Func<IFormRecognizerClient, Task> action, int formRecognizerId, CancellationToken cancellationToken)
        {
            target = await formRecognizerClientService.GetAsync(formRecognizerId, cancellationToken);

            await action(target);
        }

        private async Task<TResult> ExecuteOnSpecificResourceAsync<TResult>(Func<IFormRecognizerClient, Task<TResult>> action, int formRecognizerId, CancellationToken cancellationToken) 
        {
            target = await formRecognizerClientService.GetAsync(formRecognizerId, cancellationToken);

            return await action(target);
        }

        private async Task<TResult> ExecuteOnAnyResourceAsync<TResult>(Func<IFormRecognizerClient, Task<TResult>> action, CancellationToken cancellationToken) 
        {
            OperationResult<TResult> result = null;

            do
            {
                target = await formRecognizerClientService.GetActiveAsync(cancellationToken);
                result = await ExecuteActionAsync(action, target, cancellationToken);
            }
            while (!result.IsSuccessful);

            return result.Data;
        }

        private async Task<OperationResult<TResult>> ExecuteActionAsync<TResult>(Func<IFormRecognizerClient, Task<TResult>> action, IFormRecognizerClient client, CancellationToken cancellationToken)
        {
            try
            {
                var result = await action(client);

                return OperationResult<TResult>.Success(result);
            }
            catch (Exception exception) 
            {
                if (string.IsNullOrWhiteSpace(exception.Message) || !exception.Message.Contains(modelCountExceededLimitMessage))
                {
                    throw;
                }

                await formRecognizerClientService.DisableAsync(client.FormRecognizerId, cancellationToken);

                return OperationResult<TResult>.Failed;
            }
        }

        private IFormRecognizerClient target;
        private readonly IFormRecognizerClientService formRecognizerClientService;
        private const string modelCountExceededLimitMessage = "\"code\":\"1014\"";
    }
}
