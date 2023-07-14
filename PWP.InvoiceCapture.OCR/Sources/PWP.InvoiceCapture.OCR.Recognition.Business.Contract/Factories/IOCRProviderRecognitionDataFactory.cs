using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories
{
    public interface IOCRProviderRecognitionDataFactory
    {
        OCRProviderRecognitionData Create(List<FieldTargetField> fieldTargetFields, OCRProviderResponse ocrProviderResponse);
    }
}
