namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class KeyValuePair
    {
        public Key Key { get; set; }
        public Value Value { get; set; }
        public double Confidence { get; set; }
    }
}
