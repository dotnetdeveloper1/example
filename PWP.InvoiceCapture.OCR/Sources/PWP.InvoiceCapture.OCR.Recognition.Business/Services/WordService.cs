using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    internal class WordService : IWordService
    {
        public float GetHorizontalDistanceFrom(WordDefinition word1, WordDefinition word2, bool midPointCalculation = false)
        {
            return midPointCalculation 
                ? Math.Abs(word1.HorizontalMidPoint - word2.HorizontalMidPoint) 
                : Math.Abs((word1.DocumentLevelNormalizedLeft - word2.DocumentLevelNormalizedRight));
        }

        public float GetClosestMatchingKeywordDistance(WordDefinition word, IEnumerable<string> keywords, IEnumerable<WordDefinition> targetWords)
        {
            foreach (var keyword in keywords)
            {
                var matchingWords = targetWords
                    .Where(targetWord => targetWord.Text.ToLower().Equals(keyword.ToLower()))
                    .ToList();

                if (matchingWords != null && matchingWords.Count > 0)
                {
                    var closest = matchingWords.OrderBy(matchingWord => GetHorizontalDistanceFrom(word, matchingWord)).FirstOrDefault();

                    return GetHorizontalDistanceFrom(word,closest);
                }
            }

            return -1;
        }
    }
}
