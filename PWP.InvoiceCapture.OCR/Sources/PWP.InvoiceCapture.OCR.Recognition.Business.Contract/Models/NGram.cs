using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class NGram
    {
        public string Text
        {
            get
            {
                return String.Join(" ", words.Select(word => word.Text).ToArray());
            }
        }
        public List<WordDefinition> Words
        {
            get
            {
                return words;
            }
        }
        public WordDefinition NextWord { get; set; }

        public void AddWord(WordDefinition word)
        {
            this.words.Add(word);
        }

        private readonly List<WordDefinition> words = new List<WordDefinition>();
    }
}
