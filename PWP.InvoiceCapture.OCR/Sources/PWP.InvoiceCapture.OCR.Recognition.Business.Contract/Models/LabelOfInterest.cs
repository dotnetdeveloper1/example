using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class LabelOfInterest
    {
        public LabelOfInterest()
        {
            Synonyms =  new List<LabelSynonym>();
            MockedErrors = new List<string>();
            ExpectedType = DataType.Undefined; 
            Keywords = new List<LabelKeyWord>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public List<LabelSynonym> Synonyms { get; set; }
        public List<LabelOfInterest> FallBackLabels { get; set; }
        public List<string> MockedErrors { get; set; }
        public DataType ExpectedType { get; set; } 
        public bool UseAbsoluteComparison { get; set; }
        public List<LabelKeyWord> Keywords { get; set; }
    }
}
