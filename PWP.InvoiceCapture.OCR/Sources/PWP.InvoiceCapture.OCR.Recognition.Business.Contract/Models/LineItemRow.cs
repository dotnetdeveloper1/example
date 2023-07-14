namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class LineItemRow
    {
        public LineItem ItemNumber { get; set; }
        public LineItem Description { get; set; }
        public LineItem Quantity { get; set; }
        public LineItem UnitPrice { get; set; }
        public LineItem TotalPrice { get; set; }
    }
}
