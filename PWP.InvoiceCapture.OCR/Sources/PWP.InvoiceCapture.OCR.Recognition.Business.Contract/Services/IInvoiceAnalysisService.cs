using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface IInvoiceAnalysisService
    {
        AnalysisResult AnalyzeOCROutput(List<FieldTargetField> fieldTargetFields, OCRElements input);
    }
}
