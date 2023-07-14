using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.Models;
using System;

namespace PWP.InvoiceCapture.OCR.Core.Services
{
    public class InvoiceTemplateService : IInvoiceTemplateService
    {
        public bool AreFeaturesOfSameTemplate(GeometricFeatureCollection feature1, GeometricFeatureCollection feature2)
        {
            Guard.IsNotNull(feature1, nameof(feature1));
            Guard.IsNotNull(feature2, nameof(feature2));

            var score = GetTemplateScore(feature1, feature2);
            var closeEnough = score.Total < totalErrorMargin && score.Height < heightErrorMargin;

            return closeEnough;
        }

        public TemplateScore GetTemplateScore(GeometricFeatureCollection feature1, GeometricFeatureCollection feature2)
        {
            var connectednessScore = ((float)Math.Abs(feature1.ConnectedComponentCount - feature2.ConnectedComponentCount)) / Math.Max(feature1.ConnectedComponentCount, feature2.ConnectedComponentCount);
            var contourScore = ((float)Math.Abs(feature1.ContourCount - feature2.ContourCount)) / Math.Max(feature1.ContourCount, feature2.ContourCount);
            var densityScore = (((double)Math.Abs(feature1.PixelDensity - feature2.PixelDensity))) / (Math.Max(feature1.PixelDensity, feature2.PixelDensity));
            var heightScore = (((float)Math.Abs(feature1.AverageBlobHeight - feature2.AverageBlobHeight))) / (float)(Math.Max(feature1.AverageBlobHeight, feature2.AverageBlobHeight));
            var totalScore = (connectednessScore + contourScore + densityScore + heightScore) / 4;

            return new TemplateScore
            {
                Connectedness = connectednessScore,
                Contour = contourScore,
                Density = densityScore,
                Height = heightScore,
                Total = totalScore
            };
        }

        private readonly float totalErrorMargin = 0.10f;
        private readonly float heightErrorMargin = 0.06f;
    }
}
