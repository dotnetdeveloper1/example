using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories
{
    public interface ITrainingBlobRepository
    {
        Task<bool> CreateBlobContainerAsync(string containerName, CancellationToken cancellationToken);
        Task<int> GetBlobsCountAsync(string containerName, CancellationToken cancellationToken);
        Task UploadAsync(string containerName, string fileId, Stream documentStream, CancellationToken cancellationToken);
        Task DeleteAsync(string containerName, string fileId, CancellationToken cancellationToken);
        Task DeleteAllBlobsAsync(string containerName, CancellationToken cancellationToken);
        string GetSasUri(string containerName);
        Task CreateTemporaryFileAsync(string fileId, Stream documentStream, CancellationToken cancellationToken);
        Task MoveTemporaryFileAsync(string fileId, string targetContainerName, CancellationToken cancellationToken);
        Task CreateTroubleShootingFileAsync(string fileId, Stream documentStream, CancellationToken cancellationToken);
        Task DownloadBlobToFolderAsync(string containerName, string folder, CancellationToken cancellationToken);
    }
}
