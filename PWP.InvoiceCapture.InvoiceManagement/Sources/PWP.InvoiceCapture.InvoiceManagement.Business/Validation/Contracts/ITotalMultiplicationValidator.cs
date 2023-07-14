using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts
{
    internal interface ITotalMultiplicationValidator : IValidator<Annotation[], Annotation, CultureInfo, Dictionary<string, string>>
    {
    }
}
