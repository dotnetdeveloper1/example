using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories
{
    public class TrainingBlobRepository : ITrainingBlobRepository
    {
        public TrainingBlobRepository(IOptions<DocumentStorageOptions> options, IFileNameProvider fileNameProvider)
        {
            Guard.IsNotNull(fileNameProvider, nameof(fileNameProvider));
            documentStorageOptions = options.Value;
            blobClient = CloudStorageAccount
                .Parse(documentStorageOptions.BlobConnectionString)
                .CreateCloudBlobClient();
            blobRequestOptions = GetBlobRequestOptions();
            this.fileNameProvider = fileNameProvider;
        }

        public async Task<bool> CreateBlobContainerAsync(string containerName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(containerName, nameof(containerName));
            var container = blobClient.GetContainerReference(containerName);
            // Create the container if it does not already exist.
            return await container.CreateIfNotExistsAsync(cancellationToken);
        }

        public async Task<int> GetBlobsCountAsync(string containerName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(containerName, nameof(containerName));

            var count = 0;
            var container = blobClient.GetContainerReference(containerName);

            BlobContinuationToken continuationToken = null;

            do
            {
                var resultSegment = await container.ListBlobsSegmentedAsync(continuationToken, cancellationToken);
                
                count += resultSegment.Results.Count();
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            return count;
        }

        public string GetSasUri(string containerName)
        {
            Guard.IsNotNullOrEmpty(containerName, nameof(containerName));
            var container = blobClient.GetContainerReference(containerName);
            return GetSharedAccessUri(container, TimeSpan.FromSeconds(documentStorageOptions.LinkTimeToLiveInSeconds));
        }

        public async Task UploadAsync(string containerName, string fileId, Stream documentStream, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(containerName, nameof(containerName));
            Guard.IsNotNullOrEmpty(fileId, nameof(fileId));
            Guard.IsNotNull(documentStream, nameof(documentStream));

            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileId);
            await blob.UploadFromStreamAsync(documentStream, null, blobRequestOptions, null, cancellationToken);
        }

        public async Task CreateTroubleShootingFileAsync(string fileId, Stream documentStream, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(fileId, nameof(fileId));
            Guard.IsNotNull(documentStream, nameof(documentStream));
            var container = fileNameProvider.GetTroubleShootingContainerName();
            await CreateSpecialFileAsync(fileId, container, documentStream, cancellationToken);
        }

        public async Task CreateTemporaryFileAsync(string fileId, Stream documentStream, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(fileId, nameof(fileId));
            Guard.IsNotNull(documentStream, nameof(documentStream));
            var container = fileNameProvider.GetTemporaryFileContainerName();
            var fileName = fileNameProvider.CreateTemporaryFileName(fileId);
            await CreateSpecialFileAsync(fileName, container, documentStream, cancellationToken);
        }

        public async Task MoveTemporaryFileAsync(string fileId, string targetContainerName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(fileId, nameof(fileId));
            Guard.IsNotNullOrEmpty(targetContainerName, nameof(targetContainerName));
            var sourceContainer = blobClient.GetContainerReference(fileNameProvider.GetTemporaryFileContainerName());
            var targetContainer = blobClient.GetContainerReference(targetContainerName);
            var blobName = fileNameProvider.CreateTemporaryFileName(fileId);
            var sourceBlob = sourceContainer.GetBlockBlobReference(blobName);
            var targetBlob = targetContainer.GetBlockBlobReference(blobName);
            await targetBlob.StartCopyAsync(sourceBlob, cancellationToken);
        }



        public async Task DeleteAllBlobsAsync(string containerName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(containerName, nameof(containerName));
            var container = blobClient.GetContainerReference(containerName);
            BlobContinuationToken continuationToken = null;

            do
            {
                var resultSegment = await container.ListBlobsSegmentedAsync(continuationToken);
                foreach (var segment in resultSegment.Results)
                {
                    await ((CloudBlockBlob)segment).DeleteIfExistsAsync();
                }
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }

        public async Task DownloadBlobToFolderAsync(string containerName, string folder, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(containerName, nameof(containerName));
            var container = blobClient.GetContainerReference(containerName);
            BlobContinuationToken continuationToken = null;

            do
            {
                var resultSegment = await container.ListBlobsSegmentedAsync(continuationToken);
                foreach (var segment in resultSegment.Results)
                {
                    var item = ((CloudBlockBlob)segment);
                    var downloadPath = Path.Combine(folder, item.Name);
                    await item.DownloadToFileAsync(downloadPath, FileMode.Create, cancellationToken);
                }
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }

        public async Task DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(containerName, nameof(containerName));
            Guard.IsNotNullOrEmpty(blobName, nameof(blobName));
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(blobName);
            await blob.DeleteIfExistsAsync(cancellationToken);
        }

        private BlobRequestOptions GetBlobRequestOptions()
        {
            var blobRetryInterval = TimeSpan.FromSeconds(documentStorageOptions.BlobRetryIntervalInSeconds);

            return new BlobRequestOptions()
            {
                RetryPolicy = new ExponentialRetry(blobRetryInterval, documentStorageOptions.BlobRetryAttempts)
            };
        }
        private string GetSharedAccessUri(CloudBlobContainer blobContainer, TimeSpan timeToLive)
        {
            var sharedAccessBlobPolicy = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.Add(timeToLive),
                Permissions = (SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List |
                                                  SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create |
                                                  SharedAccessBlobPermissions.Delete | SharedAccessBlobPermissions.Add)
            };
            var sharedAccessSignature = blobContainer.GetSharedAccessSignature(sharedAccessBlobPolicy, null);
            return $"{blobContainer.Uri}{sharedAccessSignature}";
        }

        private async Task CreateSpecialFileAsync(string fileId, string containerName, Stream documentStream, CancellationToken cancellationToken)
        {
            await CreateBlobContainerAsync(containerName, cancellationToken);
            await UploadAsync(containerName, fileId, documentStream, cancellationToken);
        }

        private readonly DocumentStorageOptions documentStorageOptions;
        private readonly CloudBlobClient blobClient;
        private readonly BlobRequestOptions blobRequestOptions;
        private readonly IFileNameProvider fileNameProvider;
    }
}
