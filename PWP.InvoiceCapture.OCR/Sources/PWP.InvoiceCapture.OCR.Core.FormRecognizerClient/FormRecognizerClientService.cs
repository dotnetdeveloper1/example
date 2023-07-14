using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client
{
    internal class FormRecognizerClientService : IFormRecognizerClientService
    {
        public FormRecognizerClientService(
            IFormRecognizerResourceRepository repository,
            ISerializationService serializationService,
            IOptions<FormRecognizerClientPoolOptions> options) 
        {
            Guard.IsNotNull(repository, nameof(repository));
            Guard.IsNotNull(serializationService, nameof(serializationService));
            Guard.IsNotNull(options, nameof(options));

            this.repository = repository;
            this.serializationService = serializationService;
            this.options = options.Value;
        }

        public async Task<IFormRecognizerClient> GetActiveAsync(CancellationToken cancellationToken)
        {
            var active = await repository.GetActiveAsync(cancellationToken);

            if (active == null)
            {
                throw new InvalidOperationException("There is no any active Form Recognizer resource available.");
            }

            return CreateFormRecognizerClient(active.Id);
        }

        public Task<IFormRecognizerClient> GetAsync(int formRecognizerId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(formRecognizerId, nameof(formRecognizerId));

            var client = CreateFormRecognizerClient(formRecognizerId);

            return Task.FromResult(client);
        }

        public Task DisableAsync(int formRecognizerId, CancellationToken cancellationToken) =>
            repository.DisableAsync(formRecognizerId, cancellationToken);

        private IFormRecognizerClient CreateFormRecognizerClient(int id) 
        {
            var clientOptions = options.FormRecognizerClientOptions[id - 1];

            return new FormRecognizerClient(clientOptions, serializationService, id);
        }

        private readonly IFormRecognizerResourceRepository repository;
        private readonly ISerializationService serializationService;
        private readonly FormRecognizerClientPoolOptions options;
    }
}
