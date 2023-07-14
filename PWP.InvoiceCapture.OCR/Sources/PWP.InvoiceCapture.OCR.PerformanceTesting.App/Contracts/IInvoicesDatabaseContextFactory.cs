using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Database;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts
{
    internal interface IInvoicesDatabaseContextFactory
    {
        InvoicesDatabaseContext Create();
    }
}
