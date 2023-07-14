using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Services;
using System.Text.RegularExpressions;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.Services
{
    internal class EmailService : IEmailService
    {
        public string FindEmail(string text)
        {
            Guard.IsNotNullOrWhiteSpace(text, nameof(text));

            return GetFirstEmailFromText(text);
        }

        private string GetFirstEmailFromText(string text)
        {
            var emailMatches = emailRegex.Matches(text);

            if (emailMatches.Count > 0)
            {
                return emailMatches[0].Value;
            }

            return string.Empty;
        }

        private readonly Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
    }
}
