using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class Table
    {
        public string Id { get; set; }
        public int PageNumber { get; set; }
        public int ColumnsCount { get; set; }
        public int RowsCount { get; set; }
        public List<TableCell> Cells { get; set; }
    }
}
