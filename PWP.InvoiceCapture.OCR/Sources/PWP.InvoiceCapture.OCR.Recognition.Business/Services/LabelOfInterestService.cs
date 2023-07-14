using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    internal class LabelOfInterestService : ILabelOfInterestService
    {
        public bool DoesWordConform(LabelOfInterest label, string word)
        {
            bool result = true;
            
            if (label.UseAbsoluteComparison)
            {
                result = result && word.GetAlphaNumericValue().ToLower().Equals(label.Text.GetAlphaNumericValue());
            }
            else
            {
                result = result && word.GetIdentifier().Equals(label.Text.GetIdentifier());
            }

            if (!result && label.Synonyms != null)
            {
                if (label.UseAbsoluteComparison)
                {
                    result = label.Synonyms.Any(x => x.Text.ToLower().GetAlphaNumericValue().Equals(word.GetAlphaNumericValue().ToLower()));
                }
                else
                {
                    result = label.Synonyms.Any(x => x.Text.GetIdentifier().Equals(word.GetIdentifier()));
                }
            }

            if (!result && label.MockedErrors != null)
            {
                if (label.UseAbsoluteComparison)
                {
                    result = label.MockedErrors.Any(x => x.ToLower().GetAlphaNumericValue().Equals(word.GetAlphaNumericValue().ToLower()));
                }
                else
                {
                    result = label.MockedErrors.Any(x => x.GetIdentifier().Equals(word.GetIdentifier()));
                }
            }

            if (!result && label.FallBackLabels != null)
            {
                result = label.FallBackLabels.Any(fallBackLabel => DoesWordConform(fallBackLabel, word));
            }

            return result;
        }
    }
}
