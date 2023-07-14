using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using PWP.InvoiceCapture.Document.Business.Contract.Models;
using PWP.InvoiceCapture.Document.Business.Contract.Repositories;
using System.Threading;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace PWP.InvoiceCapture.Document.DataAccess.Repositories
{
    internal class AzureBlobRepository : IDocumentRepository
    {
        public AzureBlobRepository(IOptions<DocumentStorageOptions> options)
        {
            documentStorageOptions = options.Value;
            container = CloudStorageAccount
                .Parse(documentStorageOptions.BlobConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(documentStorageOptions.BlobContainer);

            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            blobRequestOptions = GetBlobRequestOptions();
        }

        public async Task<string> SaveAsync(CreateDocumentArgs createDocumentArgs, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(createDocumentArgs, nameof(createDocumentArgs));
            Guard.IsNotNullOrWhiteSpace(createDocumentArgs.FileId, nameof(createDocumentArgs.FileId));

            var blob = GetBlob(createDocumentArgs.FileId);

            await blob.UploadFromStreamAsync(createDocumentArgs.FileContent, null, blobRequestOptions, null, cancellationToken);

            return createDocumentArgs.FileId;
        }

        public async Task<GetDocumentStreamResult> GetStreamAsync(string fileId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));

            var blob = GetBlob(fileId);

            if (!await blob.ExistsAsync(blobRequestOptions, null, cancellationToken))
            {
                return null;
            }

            await blob.FetchAttributesAsync();

            return new GetDocumentStreamResult
            {
                Length = blob.Properties.Length,
                ContentType = blob.Properties.ContentType,
                FileStream = await blob.OpenReadAsync(null, blobRequestOptions, null, cancellationToken)
            };
        }

        public async Task<string> GetTemporaryLinkAsync(string fileId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));

            var blob = GetBlob(fileId);

            if (!await blob.ExistsAsync(blobRequestOptions, null, cancellationToken))
            {
                return null;
            }

            return GetSharedAccessUri(blob, TimeSpan.FromSeconds(documentStorageOptions.LinkTimeToLiveInSeconds));
        }

        private BlobRequestOptions GetBlobRequestOptions()
        {
            var blobRetryInterval = TimeSpan.FromSeconds(documentStorageOptions.BlobRetryIntervalInSeconds);

            return new BlobRequestOptions()
            {
                RetryPolicy = new ExponentialRetry(blobRetryInterval, documentStorageOptions.BlobRetryAttempts)
            };
        }

        private CloudBlockBlob GetBlob(string path)
        {
            var blobPath = ConvertToBlobPath(path);

            return container.GetBlockBlobReference(blobPath);
        }

        private static string ConvertToBlobPath(string path)
        {
            return path == null
                ? string.Empty
                : path.Replace('\\', '/');
        }

        private string GetSharedAccessUri(CloudBlockBlob blob, TimeSpan timeToLive)
        {
            var sharedAccessBlobPolicy = new SharedAccessBlobPolicy();
            sharedAccessBlobPolicy.SharedAccessExpiryTime = DateTime.UtcNow.Add(timeToLive);
            sharedAccessBlobPolicy.Permissions = SharedAccessBlobPermissions.Read;
            var sharedAccessSignature = blob.GetSharedAccessSignature(sharedAccessBlobPolicy);

            return $"{blob.Uri}{sharedAccessSignature}";
        }

        private readonly BlobRequestOptions blobRequestOptions;
        private readonly CloudBlobContainer container;
        private readonly DocumentStorageOptions documentStorageOptions;
    }
}
