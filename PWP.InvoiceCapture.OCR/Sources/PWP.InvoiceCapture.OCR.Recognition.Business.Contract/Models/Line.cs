using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class Line
    {
        public Line()
        {
            Words = new List<WordDefinition>();
        }

        public string Text
        {
            get
            {
                var wordList = Words.Select(word => word.Text).ToArray();
                return string.Join(" ", wordList);
            }
        }

        public float DocumentLevelNormalizedLeft => Words.First().DocumentLevelNormalizedLeft;
        
        public float DocumentLevelNormalizedRight => Words.Last().DocumentLevelNormalizedRight;
        
        public float DocumentLevelNormalizedTop => Words.First().DocumentLevelNormalizedTop;
        
        public float DocumentLevelNormalizedBottom => Words.Last().DocumentLevelNormalizedBottom;

        public float PageLevelNormalizedLeft => Words.First().PageLevelNormalizedLeft;
        
        public float PageLevelNormalizedRight => Words.Last().PageLevelNormalizedRight;

        public float PageLevelNormalizedTop => Words.First().PageLevelNormalizedTop;
        
        public float PageLevelNormalizedBottom => Words.Last().PageLevelNormalizedBottom;

        public int PageNumber => Words.First().PageNumber;

        public string Signature => String.Join("", ProcessedWords.Select(w => (int)w.DataType).ToArray());

        public List<WordDefinition> Words { get; set; }
        public List<WordDefinition> ProcessedWords { get; set; }
        public List<WordDefinition> CleanWords { get; set; }
        public List<NGram> Ngrams { get; set; }
        public bool HasAddress { get; set; }
        public int PartCount { get; set; }
        public SortedSet<float> Spaces { get; set; } = new SortedSet<float>();
        public bool BasicDataFound { get; set; }
    }
}
