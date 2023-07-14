using System;
using System.Collections.Generic;
using System.Text;

namespace PWP.InvoiceCapture.OCR.Core.Contracts
{
    public interface IFileNameProvider
    {
        string CreateTemporaryFileName(string fileId);
        string GetBlobContainerName(string templateId);
        string GetTemporaryFileContainerName();
        string GetTrainingFileSuffix();
        string GetTroubleShootingContainerName();
    }
}
