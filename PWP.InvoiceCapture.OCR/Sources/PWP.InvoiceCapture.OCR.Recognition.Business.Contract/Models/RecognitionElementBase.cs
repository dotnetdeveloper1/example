namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class RecognitionElementBase
    {
        public int Id { get; set; }
        public float PageLevelNormalizedLeft { get; set; }
        public float PageLevelNormalizedRight { get; set; }
        public float PageLevelNormalizedTop { get; set; }
        public float PageLevelNormalizedBottom { get; set; }
    }
}
