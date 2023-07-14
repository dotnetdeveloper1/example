using System.Drawing;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface ICalculationService
    {
        int CalculateHeight(int newWidth, SizeF previousSize);
    }
}
