using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts
{
    internal interface IDataAnnotationComparisonService
    {
        public InvoiceRecognitionResult GetComparisonResult(DataAnnotation expectedAnnotation, DataAnnotation actualAnnotation);
    }
}
