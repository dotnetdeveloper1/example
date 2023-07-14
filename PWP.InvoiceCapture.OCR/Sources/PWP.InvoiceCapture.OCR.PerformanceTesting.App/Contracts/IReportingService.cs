using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts
{
    internal interface IReportingService
    {
        string Extension { get; }
        byte[] Create(RecognitionTestResult results);
    }
}
