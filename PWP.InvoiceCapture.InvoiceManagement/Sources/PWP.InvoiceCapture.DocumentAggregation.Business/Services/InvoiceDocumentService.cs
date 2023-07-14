using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Services;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.Services
{
    internal class InvoiceDocumentService : IInvoiceDocumentService
    {
        public InvoiceDocumentService(IDocumentApiClient documentApiClient, IServiceBusPublisher publisher, IApplicationContext applicationContext) 
        {
            Guard.IsNotNull(documentApiClient, nameof(documentApiClient));
            Guard.IsNotNull(publisher, nameof(publisher));
            Guard.IsNotNull(applicationContext, nameof(applicationContext));

            this.documentApiClient = documentApiClient;
            this.publisher = publisher;
            this.applicationContext = applicationContext;
        }

        public async Task<UploadedDocument> SaveAsync(Stream fileContent, string fileName, FileSourceType sourceType, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(fileContent, nameof(fileContent));
            Guard.IsNotNullOrWhiteSpace(fileName, nameof(fileName));

            var documentId = await UploadDocumentAsync(fileContent, fileName, cancellationToken);

            await PublishMessageAsync(documentId, fileName, sourceType, cancellationToken);

            return new UploadedDocument { DocumentId = documentId };
        }

        public async Task<UploadedDocument> SaveEmailDocumentAsync(string to, string from, string fileName, Stream fileStream, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(fileStream, nameof(fileStream));
            Guard.IsNotNullOrWhiteSpace(to, nameof(to));
            Guard.IsNotNullOrWhiteSpace(from, nameof(from));
            Guard.IsNotNullOrWhiteSpace(fileName, nameof(fileName));

            var uploadedDocumentId = await UploadDocumentAsync(fileStream, fileName, cancellationToken);

            await PublishEmailMessageAsync(to, from, uploadedDocumentId, fileName, cancellationToken);

            return new UploadedDocument { DocumentId = uploadedDocumentId };
        }

        private async Task<string> UploadDocumentAsync(Stream fileContent, string fileName, CancellationToken cancellationToken) 
        {
            var uploadResult = await documentApiClient.UploadDocumentAsync(fileContent, fileName, cancellationToken);

            return uploadResult.Data.FileId;
        }

        private async Task PublishMessageAsync(string fileId, string fileName, FileSourceType sourceType, CancellationToken cancellationToken) 
        {
            var message = new InvoiceDocumentUploadedMessage
            {
                FileId = fileId,
                FileName = fileName,
                FileSourceType = sourceType,
                TenantId = applicationContext.TenantId,
                CultureName = applicationContext.Culture
            };

            await publisher.PublishAsync(message, cancellationToken);
        }

        private async Task PublishEmailMessageAsync(string to, string from, string fileId, string fileName, CancellationToken cancellationToken)
        {
            var message = new EmailDocumentUploadedMessage
            {
                To = to,
                From = from,
                FileId = fileId,
                FileName = fileName,
                FileSourceType = FileSourceType.Email
            };

            await publisher.PublishAsync(message, cancellationToken);
        }

        private readonly IDocumentApiClient documentApiClient;
        private readonly IServiceBusPublisher publisher;
        private readonly IApplicationContext applicationContext;
    }
}
