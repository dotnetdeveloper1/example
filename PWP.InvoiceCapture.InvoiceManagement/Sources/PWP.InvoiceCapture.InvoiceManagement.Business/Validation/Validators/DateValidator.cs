using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System;
using System.Data.SqlTypes;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class DateValidator : IDateValidator
    {
        public ValidationResult Validate(Annotation entity, CultureInfo cultureInfo, string fieldName)
        {
            Guard.IsNotNull(entity, nameof(entity));
            Guard.IsNotNull(entity.FieldValue, nameof(entity.FieldValue));
            Guard.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));

            DateTime parsedDate;
            var isValid = DateTime.TryParse(entity.FieldValue, cultureInfo, DateTimeStyles.None, out parsedDate);

            if (!isValid)
            {
                return ValidationResult.Failed($"Field {fieldName} can't be converted to date.");
            }

            if (parsedDate < SqlDateTime.MinValue.Value || parsedDate > SqlDateTime.MaxValue.Value)
            {
                return ValidationResult.Failed($"Field {fieldName} must be between 1/1/1753 and 12/31/9999.");
            }

            return ValidationResult.Ok;
        }

    }
}
