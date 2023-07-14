using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class LineOrderValidator : ILineOrderValidator
    {
        public ValidationResult Validate(List<LineAnnotation> invoiceLineAnnotations)
        {
            Guard.IsNotNull(invoiceLineAnnotations, nameof(invoiceLineAnnotations));

            var isZeroOrLess = invoiceLineAnnotations.Any(lineAnnotation => lineAnnotation.OrderNumber < 1);
            
            if (isZeroOrLess)
            {
                return ValidationResult.Failed("OrderNumber should be greater then 0.");
            }

            var distinctOrderNumbersCount = invoiceLineAnnotations
                .Select(line => line.OrderNumber)
                .Distinct()
                .Count();

            if (distinctOrderNumbersCount != invoiceLineAnnotations.Count())
            {
                return ValidationResult.Failed("InvoiceLineAnnotations contains OrderNumbers with the same values.");
            }

            return ValidationResult.Ok;
        }
    }

}
