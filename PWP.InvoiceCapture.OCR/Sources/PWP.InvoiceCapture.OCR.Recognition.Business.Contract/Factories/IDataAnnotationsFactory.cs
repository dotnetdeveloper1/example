using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories
{
    public interface IDataAnnotationsFactory
    {
        DataAnnotation Create(OCRElements ocrElements, AnalysisResult analysisResult);        
    }
}
