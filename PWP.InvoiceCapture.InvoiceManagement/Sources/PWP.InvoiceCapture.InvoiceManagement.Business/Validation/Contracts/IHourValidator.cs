using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts
{
    internal interface IHourValidator : IValidator<Annotation, CultureInfo, string>
    {
    }
}
