using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface IRegionService
    {
        bool DoesWordBelong(WordDefinition word, Region region);
    }
}
