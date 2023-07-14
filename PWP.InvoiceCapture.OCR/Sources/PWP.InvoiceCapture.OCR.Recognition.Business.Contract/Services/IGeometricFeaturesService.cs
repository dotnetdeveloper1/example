using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.OCR.Core.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface IGeometricFeaturesService
    {
        OperationResult<GeometricFeatureCollection> ProcessImage(byte[] imageBytes);
    }
}
