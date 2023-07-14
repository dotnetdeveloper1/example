using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class TableDefinitionCell : RecognitionElementBase
    {
        public int ColumnIndex { get; set; }
        public int RowIndex { get; set; }
        public int? ColumnSpan { get; set; }
        public int? RowSpan { get; set; }
        public string Text { get; set; }   
        public List<string> WordIds { get; set; }
    }
}
