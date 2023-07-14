using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class WordDefinition : RecognitionElementBase, ICloneable
    {
        public WordDefinition()
        {
            LineNo = -1;
            Id = -1;
        }

        // if this is a composite word, this list will hold the ids of invidividual words contained in this.
        public List<int> IndividualWordIds { get; set; } = new List<int>();

        public float HorizontalMidPoint => (DocumentLevelNormalizedLeft + DocumentLevelNormalizedRight) / 2;

        public RectangleF Rectangle => new RectangleF(DocumentLevelNormalizedLeft, DocumentLevelNormalizedBottom, DocumentLevelNormalizedRight - DocumentLevelNormalizedLeft, DocumentLevelNormalizedBottom - DocumentLevelNormalizedTop);

        public RectangleF NormalizedRectangle => new RectangleF(PageLevelNormalizedLeft, PageLevelNormalizedTop, PageLevelNormalizedRight - PageLevelNormalizedLeft, PageLevelNormalizedBottom - PageLevelNormalizedTop);

        public float ValueProbability
        {
            get
            {
                if (DataType == DataType.Date || DataType == DataType.Number)
                {

                    return 1;
                }
                else
                {
                    var digits = Text.ToCharArray().Where(x => char.IsDigit(x)).Count();

                    return (float)digits / Text.Length;
                }
            }
        }

        public float Width => DocumentLevelNormalizedRight - DocumentLevelNormalizedLeft;

        public float Height => DocumentLevelNormalizedBottom - DocumentLevelNormalizedTop;

        public int LineNo { get; set; }
        public int PageNumber { get; set; }
        public float DocumentLevelNormalizedLeft { get; set; }
        public float DocumentLevelNormalizedRight { get; set; }
        public float DocumentLevelNormalizedTop { get; set; }
        public float DocumentLevelNormalizedBottom { get; set; }

        public float PageLevelPixelsLeft { get; set; }
        public float PageLevelPixelsRight { get; set; }
        public float PageLevelPixelsTop { get; set; }
        public float PageLevelPixelsBottom { get; set; }

        public int BlobId { get; set; }
        public string Text { get; set; }
        public string ReconstructedText { get; set; }
        public DataType DataType { get; set; }
        public int DistanceToNext { get; set; }
        public Region Region { get; set; }
        public bool IsLabel { get; set; } = false;

        public object Clone()
        {
            return new WordDefinition
            {
                LineNo = LineNo,
                DocumentLevelNormalizedLeft = DocumentLevelNormalizedLeft,
                DocumentLevelNormalizedRight = DocumentLevelNormalizedRight,
                DocumentLevelNormalizedTop = DocumentLevelNormalizedTop,
                DocumentLevelNormalizedBottom = DocumentLevelNormalizedBottom,
                BlobId = BlobId,
                PageLevelNormalizedLeft = PageLevelNormalizedLeft,
                PageLevelNormalizedRight = PageLevelNormalizedRight,
                PageLevelNormalizedTop = PageLevelNormalizedTop,
                PageLevelNormalizedBottom = PageLevelNormalizedBottom,
                PageLevelPixelsLeft = PageLevelPixelsLeft,
                PageLevelPixelsRight = PageLevelPixelsRight,
                PageLevelPixelsBottom = PageLevelPixelsBottom,
                PageLevelPixelsTop = PageLevelPixelsTop,
                PageNumber = PageNumber,
                Text = Text,
                Id = Id,
                ReconstructedText = ReconstructedText,
                DataType = DataType,
                DistanceToNext = DistanceToNext,
                Region = Region,
                IsLabel = IsLabel
            };
        }
    }
}
