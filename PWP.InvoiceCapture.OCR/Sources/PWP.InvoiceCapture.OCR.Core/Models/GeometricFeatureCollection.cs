using System;

namespace PWP.InvoiceCapture.OCR.Core.Models
{
    public class GeometricFeatureCollection : ICloneable
    {
        public int Id { get; set; }
        public double PixelDensity { get; set; } = -1;
        public int ContourCount { get; set; } = -1;
        public int LineCount { get; set; } = -1;
        public double AverageBlobHeight { get; set; }
        public int ConnectedComponentCount { get; set; } = -1;
        public int InvoiceTemplateId { get; set; }
        public InvoiceTemplate InvoiceTemplate { get; set; }

        public object Clone()
        {
            var clone = (GeometricFeatureCollection)this.MemberwiseClone();
            clone.InvoiceTemplate = null;
            return clone;
        }
    }
}
