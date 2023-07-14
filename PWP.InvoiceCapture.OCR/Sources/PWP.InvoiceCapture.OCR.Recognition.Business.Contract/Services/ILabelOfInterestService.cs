using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface ILabelOfInterestService
    {
        bool DoesWordConform(LabelOfInterest label, string word);
    }
}
