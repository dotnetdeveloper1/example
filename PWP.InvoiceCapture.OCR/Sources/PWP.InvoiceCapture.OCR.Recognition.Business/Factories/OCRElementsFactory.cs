using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Factories
{
    internal class OCRElementsFactory : IOCRElementsFactory
    {
        public OCRElements Create(IEnumerable<WordDefinition> words )
        {
            var lines = new List<Line>();
            words = words.OrderBy(word => word.LineNo);
            var lastLineNo = int.MinValue;
            Line lastLine = null;
            var currentLineNo = -1;

            foreach (var word in words)
            {   
                if (word.LineNo != lastLineNo)
                {
                    if (lastLine != null)
                    {
                        // OCR providers like FormRecognizer sometimes assigns incorrect line numberings due to font size differences between words
                        // If this is the case we need to check with our line creation logic and reassign the linenumbers.
                        if(IsWordInLine(word,lastLine))
                        {
                            lastLine.Words.Add(word);
                            word.LineNo = currentLineNo;
                            continue;
                        }
                    }

                    currentLineNo++;
                    word.LineNo = currentLineNo;
                    lastLine = new Line();
                    lines.Add(lastLine);
                    lastLineNo = word.LineNo;
                }

                lastLine.Words.Add(word);
            }

            foreach(var line in lines)
            {
                line.Words = line.Words.OrderBy(x => x.DocumentLevelNormalizedLeft).ToList();
            }

            var allText = string.Join(" ",lines.Select(line => line.Text).ToArray());

            return new OCRElements { Lines = lines, Words = words, RawText = allText };
        }

        private bool IsWordInLine(WordDefinition word, Line line)
        {
            var lineCenter = (line.PageLevelNormalizedTop + line.PageLevelNormalizedBottom) / 2;

            return word.PageLevelNormalizedTop <= lineCenter && word.PageLevelNormalizedBottom >= lineCenter;
        }
    }
}
