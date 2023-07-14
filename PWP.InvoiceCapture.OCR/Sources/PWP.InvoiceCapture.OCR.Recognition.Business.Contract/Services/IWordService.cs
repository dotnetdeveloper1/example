using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface IWordService
    {
        float GetHorizontalDistanceFrom(WordDefinition word1, WordDefinition word2, bool midPointCalculation);
        float GetClosestMatchingKeywordDistance(WordDefinition word, IEnumerable<string> keywords, IEnumerable<WordDefinition> targetWords);
    }
}
