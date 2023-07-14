using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class TableCell
    {
        public int Id { get; set; }
        public int ColumnIndex { get; set; }
        public int RowIndex { get; set; }
        public int? ColumnSpan { get; set; }
        public int? RowSpan { get; set; }
        public string Text { get; set; }
        public DocumentLayoutPoint TopLeft { get; set; }
        public DocumentLayoutPoint BottomRight { get; set; }
        public List<string> DocumentLayoutItemIds { get; set; }
    }
}
