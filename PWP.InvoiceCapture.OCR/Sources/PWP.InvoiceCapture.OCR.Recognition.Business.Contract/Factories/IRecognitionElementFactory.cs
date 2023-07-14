using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories
{
    public interface IRecognitionElementFactory
    {
        List<WordDefinition> CreateWords(FormRecognizerResponse formRecognizerApiResponse);
        List<TableDefinition> CreateTables(FormRecognizerResponse formRecognizerApiResponse);
    }
}
