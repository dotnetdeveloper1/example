using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IPdfService
    {
        List<PageImage> ConvertToImages(byte[] bytes);
    }
}
