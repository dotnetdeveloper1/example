using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer
{
    public class TrainResult
    {
        public double AverageModelAccuracy { get; set; }
        public IList<TrainingDocument> TrainingDocuments { get; set; }
        public IList<Field> Fields { get; set; }
        public IList<object> Errors { get; set; }
    }
}
