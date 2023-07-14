using PWP.InvoiceCapture.OCR.Core.Models;

namespace PWP.InvoiceCapture.OCR.Core.Contracts
{
    public interface IInvoiceTemplateService
    {
        bool AreFeaturesOfSameTemplate(GeometricFeatureCollection feature1, GeometricFeatureCollection feature2);
        TemplateScore GetTemplateScore(GeometricFeatureCollection feature1, GeometricFeatureCollection feature2);
    }
}
