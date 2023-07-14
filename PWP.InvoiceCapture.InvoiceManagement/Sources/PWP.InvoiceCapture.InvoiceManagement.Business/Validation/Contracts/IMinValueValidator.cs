using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts
{
    internal interface IMinValueValidator : IValidator<Annotation, decimal, string>
    {
    }
}
