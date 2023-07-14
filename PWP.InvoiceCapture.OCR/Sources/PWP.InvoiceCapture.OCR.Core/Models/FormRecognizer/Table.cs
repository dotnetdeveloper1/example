using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class Table
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Cell> Cells { get; set; }
    }
}
