using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class EmailProvider : IEmailProvider
    {
        public EmailProvider(IOptions<EmailAddressGenerationOptions> optionsAccessor, INameGenerator nameGenerator)
        {
            GuardOptions(optionsAccessor);
            Guard.IsNotNull(nameGenerator, nameof(nameGenerator));

            options = optionsAccessor.Value;
            this.nameGenerator = nameGenerator;
        }

        public string Generate()
        {
            return nameGenerator.GenerateName().ToLower() + options.Postfix;
        }

        private void GuardOptions(IOptions<EmailAddressGenerationOptions> optionsAccessor)
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(optionsAccessor.Value, nameof(optionsAccessor.Value));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.Postfix, nameof(optionsAccessor.Value.Postfix));
        }

        private readonly EmailAddressGenerationOptions options;
        private readonly INameGenerator nameGenerator;
    }
}
