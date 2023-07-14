using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface IFormRecognizerKeyConversionService 
    {
        DocumentResultItem GetDescription(int lineNo, Dictionary<string, DocumentResultItem> fieldValues);
        DocumentResultItem GetItemNo(int lineNo, Dictionary<string, DocumentResultItem> fieldValues);
        int GetNumberOfLineItems(Dictionary<string, DocumentResultItem> fieldValues);
        DocumentResultItem GetQuantity(int lineNo, Dictionary<string, DocumentResultItem> fieldValues);
        DocumentResultItem GetTotalPrice(int lineNo, Dictionary<string, DocumentResultItem> fieldValues);
        DocumentResultItem GetUnitPrice(int lineNo, Dictionary<string, DocumentResultItem> fieldValues);
        List<KeyValuePair<string, DocumentResultItem>> GetFormItems(List<FieldTargetField> fieldTargetFields, Dictionary<string, DocumentResultItem> fieldValues);
    }
}
