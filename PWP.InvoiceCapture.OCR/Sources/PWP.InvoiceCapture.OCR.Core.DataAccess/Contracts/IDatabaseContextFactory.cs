using PWP.InvoiceCapture.OCR.Core.DataAccess;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.Contracts
{
    public interface IDatabaseContextFactory
    {
        IDatabaseContext Create();
        IDatabaseContext CreateWithTracking();
    }
}
