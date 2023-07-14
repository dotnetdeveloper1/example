using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.Models
{
    public class InvoiceTemplate
    {
        public InvoiceTemplate()
        {
            GeometricFeatures = new GeometricFeatureCollection();
        }

        public int Id { get; set; }
        public string FormRecognizerModelId { get; set; }
        public int? FormRecognizerId { get; set; }
        public int TrainingFileCount { get; set; }
        public string TrainingBlobUri { get; set; }
        /*  These features are obtained from the first page of invoice, before the OCR stage */
        public GeometricFeatureCollection GeometricFeatures { get; set; }
        public string TenantId { get; set; }
        /*  These features are obtained after the OCR stage */
        public Dictionary<string, Coordinate> KeyWordCoordinates { get; set; }
    }
}
