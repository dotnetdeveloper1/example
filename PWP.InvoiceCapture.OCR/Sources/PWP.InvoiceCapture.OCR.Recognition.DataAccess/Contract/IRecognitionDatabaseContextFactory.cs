using PWP.InvoiceCapture.OCR.Core.DataAccess;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Contracts
{
    public interface IRecognitionDatabaseContextFactory
    {
        IRecognitionDatabaseContext Create();
        IRecognitionDatabaseContext CreateWithTracking();
    }
}
