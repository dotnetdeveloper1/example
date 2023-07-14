using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class WordGroup
    {
        public WordGroup()
        {
            Words = new List<WordDefinition>();
        }

        public WordGroup(List<WordDefinition> words)
        {
            Words = words;
        }

        public WordGroup(WordDefinition word)
        {
            Words = new List<WordDefinition>();
            Words.Add(word);
        }

        public string Text
        {
            get
            {
                var texts = Words
                    .OrderBy(word => word.Id)
                    .Select(word => word.Text);

                return string.Join(" ", texts);
            }
        }

        public float ValueProbability
        {
            get
            {
                if (Words.Count == 0)
                {

                    return 0;
                }
                    
                return Words.Average(word => word.ValueProbability);
            }
        }

        public List<WordDefinition> Words { get; set; } = new List<WordDefinition>();
    }
}
