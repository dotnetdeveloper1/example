using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Enumerations;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class FieldTargetField
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public FormRecognizerFieldType FormRecognizerFieldType { get; set; }
        public string TargetFieldValue => ((int)FormRecognizerFieldType).ToString();
        public string TargetFieldName => FormRecognizerFieldType.ToString();
        public DataType DataType { get; set; }
    }
}
