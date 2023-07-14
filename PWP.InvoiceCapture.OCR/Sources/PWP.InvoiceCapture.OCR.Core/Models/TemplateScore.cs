namespace PWP.InvoiceCapture.OCR.Core.Models
{
    public class TemplateScore
    {
        public double Connectedness { get; set; }
        public double Contour { get; set; }
        public double Density { get; set; }    
        public double Height { get; set; }
        public double Total { get; set; }
    }
}
