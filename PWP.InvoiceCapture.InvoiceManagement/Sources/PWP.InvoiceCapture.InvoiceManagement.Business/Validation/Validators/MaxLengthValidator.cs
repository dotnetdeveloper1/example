using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class MaxLengthValidator : IMaxLengthValidator
    {
        public ValidationResult Validate(Annotation entity, int maxLength, string fieldName)
        {
            Guard.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));
            Guard.IsNotNull(entity, nameof(entity));
            Guard.IsNotNull(entity.FieldValue, nameof(entity.FieldValue));

            var validationResult = entity.FieldValue.Length > maxLength
                ? ValidationResult.Failed($"Field {fieldName} is more than {maxLength} characters.")
                : ValidationResult.Ok;

            return validationResult;
        }
    }
}
