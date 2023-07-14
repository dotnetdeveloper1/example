using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class OCRProviderRecognitionData
    {
        public List<FormValue> FormValues { get; set; }
        public List<LineItemRow> LineItemsRows { get; set; }
        public List<TableDefinition> Tables { get; set; }
    }
}
