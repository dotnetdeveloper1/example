using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class TableDefinition
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int ColumnsCount { get; set; }
        public int RowsCount { get; set; }
        public List<TableDefinitionCell> Cells { get; set; }
    }
}
