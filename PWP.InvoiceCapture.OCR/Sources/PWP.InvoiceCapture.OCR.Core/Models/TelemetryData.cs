namespace PWP.InvoiceCapture.OCR.Core.Models
{
    public class TelemetryData
    {
        public string TenantId { get; set; }
        public int InvoiceId { get; set; }
        public int PreOcrTemplateId { get; set; }
        public int PostOcrTemplateId { get; set; }
        public string FileId { get; set; }
        public string FormRecognizerModelId { get; set; }

        public static TelemetryData Create(int invoiceId, string tenantId, string fileId)
        {
            return new TelemetryData
            {
                TenantId = tenantId,
                FileId = fileId,
                InvoiceId = invoiceId
            };
        }
    }
}
