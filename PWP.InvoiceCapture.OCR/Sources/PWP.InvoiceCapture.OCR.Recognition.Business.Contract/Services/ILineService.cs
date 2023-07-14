using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface ILineService
    {
        List<NGram> CreateNgramsOfLine(List<WordDefinition> words);
        WordDefinition GetClosestWordInLine(WordDefinition word, Line line);
        WordDefinition GetIntersectingWordOfLine(WordDefinition word, Line line);
    }
}
