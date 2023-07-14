using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class AnalysisResult
    {
        public Dictionary<FieldTargetField, List<WordGroup>> Labels { get; set; }
        public IEnumerable<LineItemRow> LineItemsRows { get; set; } 
    }

}
