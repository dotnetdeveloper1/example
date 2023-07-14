using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Factories
{
    public interface IFormRecognizerTrainingDocumentFactory
    {
        FormReconizerTrainingDocument Create(string fileName, DataAnnotation annotations);
    }
}
