using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Mappers
{
    public interface IFieldMapper
    {
        FieldTargetField ToFieldTargetField(Field field);
        List<FieldTargetField> ToFieldTargetFieldList(List<Field> fields);
    }
}
