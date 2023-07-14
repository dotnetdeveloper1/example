using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class Region
    {
        public Region()
        {
            Left = -1;
            Top = -1;
            Right = -1;
            Bottom = -1;
        }

        public string Text
        {
            get
            {
                var words = new List<WordDefinition>(Words);
                var wordTexts = words
                    .OrderBy(word => word.LineNo)
                    .ThenBy(word => word.DocumentLevelNormalizedLeft)
                    .Select(word => word.Text + " ")
                    .ToList();

                return string.Concat(wordTexts);
            }
        }

        public List<WordDefinition> Words { get; set; } = new List<WordDefinition>();
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public bool Initialized { get; set; }
        
        public void AddWord(WordDefinition word)
        {
            if (word.Region != null)
            {
                return;
            }

            if (!Initialized)
            {
                Left = word.DocumentLevelNormalizedLeft;
                Top = word.DocumentLevelNormalizedTop;
                Initialized = true;
            }

            if (word.DocumentLevelNormalizedRight > Right)
            {
                Right = word.DocumentLevelNormalizedRight;
            }

            if (word.DocumentLevelNormalizedBottom > Bottom)
            {
                Bottom = word.DocumentLevelNormalizedBottom;
            }

            Words.Add(word);
            word.Region = this;
        }
    }
}
