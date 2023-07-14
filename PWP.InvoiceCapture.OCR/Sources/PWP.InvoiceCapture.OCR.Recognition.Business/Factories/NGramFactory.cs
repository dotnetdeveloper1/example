using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Factories
{
    internal class NGramFactory : INGramFactory
    {
        public List<NGram> Create(List<WordDefinition> words)
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
                    
                    result.Add(nGram);
                    index++;
                }
            }

            return result;
        }
    }
}
