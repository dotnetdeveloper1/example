using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class SingleFieldPerTypeValidator : ISingleFieldPerTypeValidator
    {
        public ValidationResult Validate(List<Annotation> annotations, Dictionary<string, string> fieldTypeNames)
        {
            Guard.IsNotNull(annotations, nameof(annotations));
            Guard.IsNotNull(fieldTypeNames, nameof(fieldTypeNames));

            var moreThanOnePerType = annotations
                .GroupBy(annotation => annotation.FieldType)
                .Where(group => group.Count() > 1);

            if (moreThanOnePerType.Any())
            {
                var fieldType = moreThanOnePerType.First().Key;
                var hasFieldName = fieldTypeNames.Any(field => string.Equals(field.Key.ToString().ToLower(), fieldType.ToLower()));

                if (hasFieldName)
                {
                    var duplicatedField = fieldTypeNames.First(field => string.Equals(field.Key.ToString().ToLower(), fieldType.ToLower()));
                    return ValidationResult.Failed($"Field type {duplicatedField.Value} is present more than once.");
                }

                return ValidationResult.Failed($"Field type {fieldType} is present more than once.");
            }

            return ValidationResult.Ok;
        }
    }
}
