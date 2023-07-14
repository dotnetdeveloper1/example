using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation
{
    public static class ValidationResultEnumerableExtensions
    {
        public static ValidationResult Combine(this IEnumerable<ValidationResult> results)
        {
            Guard.IsNotNull(results, nameof(results));

            var invalidResultMessages = results
                .Where(result => !result.IsValid)
                .Select(result => result.Message);

            if (invalidResultMessages.Any())
            {
                return ValidationResult.Failed(string.Join(Environment.NewLine, invalidResultMessages));
            }

            return ValidationResult.Ok;
        }
    }
}
