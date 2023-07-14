using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Contracts;

namespace PWP.InvoiceCapture.OCR.Core.Services
{
    public class FileNameProvider : IFileNameProvider
    {
        public string CreateTemporaryFileName(string fileId)
        {
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));

            return $"{fileId}.ocr.json";
        }

        public string GetBlobContainerName(string templateId) => $"{blobContainerNamePrefix}-{templateId}";

        public string GetTrainingFileSuffix() => trainingFileSuffix;

        public string GetTemporaryFileContainerName() => temporaryFileContainerName;

        public string GetTroubleShootingContainerName() => troubleShootingContainerName;

        private readonly string blobContainerNamePrefix = "fr-training-blob";
        private readonly string troubleShootingContainerName = "troubleshooting";
        private readonly string temporaryFileContainerName = "temp-files";
        private readonly string trainingFileSuffix = ".labels.json";
    }
}
