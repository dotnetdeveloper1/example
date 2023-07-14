using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts
{
    internal interface IMaxValueValidator : IValidator<Annotation, decimal, string>
    {
    }
}
