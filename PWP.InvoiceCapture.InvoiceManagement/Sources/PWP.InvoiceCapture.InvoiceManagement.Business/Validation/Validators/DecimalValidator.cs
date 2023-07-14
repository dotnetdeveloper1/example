using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class DecimalValidator : IDecimalValidator
    {
        public ValidationResult Validate(Annotation entity, CultureInfo cultureInfo, string fieldName)
        {
            Guard.IsNotNull(entity, nameof(entity));
            Guard.IsNotNull(entity.FieldValue, nameof(entity.FieldValue));
            Guard.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));

            return decimal.TryParse(entity.FieldValue, NumberStyles.Any, cultureInfo,  out _)
                ? ValidationResult.Ok
                : ValidationResult.Failed($"Field {fieldName} can't be converted to decimal.");
        }
    }
}
