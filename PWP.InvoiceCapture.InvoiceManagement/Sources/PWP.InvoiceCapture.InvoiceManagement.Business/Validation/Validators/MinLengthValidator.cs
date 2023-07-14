using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class MinLengthValidator : IMinLengthValidator
    {
        public ValidationResult Validate(Annotation entity, int minLength, string fieldName)
        {
            Guard.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));
            Guard.IsNotNull(entity, nameof(entity));
            Guard.IsNotNull(entity.FieldValue, nameof(entity.FieldValue));

            var validationResult = entity.FieldValue.Length < minLength
                ? ValidationResult.Failed($"Field {fieldName} is less than {minLength} characters.")
                : ValidationResult.Ok;

            return validationResult;
        }
    }
}
