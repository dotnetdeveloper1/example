using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    internal class LineService: ILineService
    {
        public List<NGram> CreateNgramsOfLine(List<WordDefinition> words)
        {
            var result = new List<NGram>();
            
            for (var size = 1; size <= words.Count; size++)
            {
                var index = 0;
                while (index + size <= words.Count)
                {
                    var nGram = new NGram();
                    
                    for (var i = 0; i < size; i++)
                    {
                        nGram.AddWord(words[index + i]);
                    }
                    
                    if (index + size < words.Count)
                    {
                        nGram.NextWord = words[index + size];
                    }
                    
                    result.Add(nGram);
                    index++;
                }
            }

            return result;
        }

        public WordDefinition GetClosestWordInLine(WordDefinition word, Line line)
        {
            var midPoint = CalculateMidPoint(word);

            return line.ProcessedWords
                .OrderBy(processedWord => Math.Abs(processedWord.HorizontalMidPoint - midPoint))
                .FirstOrDefault();
        }

        public WordDefinition GetIntersectingWordOfLine(WordDefinition word, Line line)
        {
            var midPoint = CalculateMidPoint(word);
            var intersectingWord = line.Words
                .Where(targetWord => targetWord.DocumentLevelNormalizedLeft < midPoint && targetWord.DocumentLevelNormalizedRight > midPoint)
                .FirstOrDefault();

            if (intersectingWord == null)
            {
                intersectingWord = line.Words
                    .Where(targetWord => IsAlignedToTheLeft(word, targetWord) || IsAlignedToTheRight(word, targetWord))
                    .FirstOrDefault();
            }

            return intersectingWord;
        }

        private bool IsAlignedToTheLeft(WordDefinition word, WordDefinition targetWord) => Math.Abs(targetWord.DocumentLevelNormalizedLeft - word.DocumentLevelNormalizedLeft) < intersectionErrorMargin;

        private bool IsAlignedToTheRight(WordDefinition word, WordDefinition targetWord) => Math.Abs(targetWord.DocumentLevelNormalizedRight - word.DocumentLevelNormalizedRight) < intersectionErrorMargin;

        private float CalculateMidPoint(WordDefinition word) => (word.DocumentLevelNormalizedLeft + word.DocumentLevelNormalizedRight) / 2;

        private const float intersectionErrorMargin = 0.005f;
    }
}
