using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories
{
    public interface INGramFactory
    {
        List<NGram> Create(List<WordDefinition> words);
    }
}
