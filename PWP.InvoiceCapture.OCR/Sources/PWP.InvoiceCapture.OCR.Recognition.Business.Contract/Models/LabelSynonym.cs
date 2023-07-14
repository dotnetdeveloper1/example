namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class LabelSynonym
    {
        public int Id { get; set; }
        public int LabelOfInterestId { get; set; }
        public string Text { get; set; }
        public bool UseInTemplateComparison { get; set; }
    }
}

