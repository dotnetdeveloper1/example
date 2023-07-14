using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IImageService
    {
        List<PageImage> ConvertToDefaultFormatImages(byte[] bytes);
    }
}
