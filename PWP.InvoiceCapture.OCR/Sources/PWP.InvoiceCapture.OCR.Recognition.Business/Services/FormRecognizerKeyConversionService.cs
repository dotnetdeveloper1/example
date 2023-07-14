using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System.Collections.Generic;
using System.Linq;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    internal class FormRecognizerKeyConversionService : IFormRecognizerKeyConversionService
    {
        public int GetNumberOfLineItems(Dictionary<string, DocumentResultItem> fieldValues)
        {
            Guard.IsNotNull(fieldValues, nameof(fieldValues));
            /* Assuming all table lines contain the total price key (not the value) */
            if (fieldValues.Keys.Any(key => key.StartsWith(totalPriceHeader)))
            {
                return fieldValues.Keys
                    .Where(key => key.StartsWith(totalPriceHeader) && fieldValues[key]!=null && !string.IsNullOrEmpty(fieldValues[key].Text))
                    .Count();
            }
            return 0;
        }

        public List<KeyValuePair<string, DocumentResultItem>> GetFormItems(List<FieldTargetField> fieldTargetFields, Dictionary<string, DocumentResultItem> fieldValues)
        {
            Guard.IsNotNull(fieldValues, nameof(fieldValues));
            return fieldValues
                .Where(fieldValue => fieldTargetFields
                    .Any(fieldTargetField => string.Equals(fieldTargetField.FieldId.ToString(), fieldValue.Key)))
                .ToList();
        }

        public DocumentResultItem GetItemNo(int lineNo, Dictionary<string, DocumentResultItem> fieldValues)
        {
            Guard.IsNotNull(fieldValues, nameof(fieldValues));
            return GetDocumentResultItem(itemNoHeader, lineNo, fieldValues);
        }

        public DocumentResultItem  GetDescription(int lineNo, Dictionary<string, DocumentResultItem> fieldValues)
        {
            Guard.IsNotNull(fieldValues, nameof(fieldValues));
            return GetDocumentResultItem(descriptionHeader, lineNo, fieldValues);
        }

        public DocumentResultItem  GetUnitPrice(int lineNo, Dictionary<string, DocumentResultItem> fieldValues)
        {
            Guard.IsNotNull(fieldValues, nameof(fieldValues));
            return GetDocumentResultItem(unitPriceHeader, lineNo, fieldValues);
        }
        public DocumentResultItem  GetQuantity(int lineNo, Dictionary<string, DocumentResultItem> fieldValues)
        {
            Guard.IsNotNull(fieldValues, nameof(fieldValues));
            return  GetDocumentResultItem(quantityHeader, lineNo, fieldValues);
        }
        public DocumentResultItem GetTotalPrice(int lineNo, Dictionary<string, DocumentResultItem> fieldValues)
        {
            Guard.IsNotNull(fieldValues, nameof(fieldValues));
            return GetDocumentResultItem(totalPriceHeader, lineNo, fieldValues);
        }
        
        private DocumentResultItem GetDocumentResultItem(string identifier, int lineNo, Dictionary<string, DocumentResultItem> fieldValues) => fieldValues.ContainsKey(identifier + (lineNo + 1).ToString()) ? fieldValues[identifier + (lineNo+1).ToString()] : null;

        private readonly string itemNoHeader = $"{InvoiceLineFieldTypes.Number}-";
        private readonly string descriptionHeader = $"{InvoiceLineFieldTypes.Description}-";
        private readonly string unitPriceHeader = $"{InvoiceLineFieldTypes.Price}-";
        private readonly string quantityHeader = $"{InvoiceLineFieldTypes.Quantity}-";
        private readonly string totalPriceHeader = $"{InvoiceLineFieldTypes.Total}-";        
    }
}
