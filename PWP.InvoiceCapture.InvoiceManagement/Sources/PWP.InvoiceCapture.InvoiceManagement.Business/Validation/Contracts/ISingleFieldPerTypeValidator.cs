using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts
{
    internal interface ISingleFieldPerTypeValidator : IValidator<List<Annotation>, Dictionary<string, string>>
    {
    }
}
