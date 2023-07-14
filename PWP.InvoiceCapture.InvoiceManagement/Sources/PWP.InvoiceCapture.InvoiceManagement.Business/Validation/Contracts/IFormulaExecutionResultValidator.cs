
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts
{
    internal interface IFormulaExecutionResultValidator
    {
        ValidationResult Validate(List<Annotation> annotations, Annotation resultAnnotation, string resultFieldName, string formula);
    }
}
