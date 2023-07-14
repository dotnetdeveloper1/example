using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts
{
    internal interface IMaxLengthValidator : IValidator<Annotation, int, string>
    {
    }
}
