using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Enumerations;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Mappers;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Mapper
{
    public class FieldMapper : IFieldMapper
    {
        public List<FieldTargetField> ToFieldTargetFieldList(List<Field> fields)
        {
            if (fields == null)
            {
                return null;
            }

            return fields.Select(field => ToFieldTargetField(field)).ToList();
        }

        public FieldTargetField ToFieldTargetField(Field field)
        {
            if (field == null)
            {
                return null;
            }

            return new FieldTargetField
            {
                FieldId = field.Id,
                FieldName = field.DisplayName,
                FormRecognizerFieldType = ConvertToRecognizerFieldType(field.TargetFieldType),
                DataType = ConvertToDataType(field.Type)
            };
        }

        private FormRecognizerFieldType ConvertToRecognizerFieldType(TargetFieldType? targetFieldType)
        {
            if (targetFieldType == null)
            {
                return FormRecognizerFieldType.Unknown;
            }

            var fieldTypeId = (int)targetFieldType.Value;
            var enumType = typeof(FormRecognizerFieldType);

            if (Enum.IsDefined(enumType, fieldTypeId))
            {
                return (FormRecognizerFieldType)Enum.ToObject(enumType, fieldTypeId);
            }

            return FormRecognizerFieldType.Unknown;
        }

        private DataType ConvertToDataType(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Decimal:
                    return DataType.Number;
                case FieldType.DateTime:
                    return DataType.Date;
                case FieldType.String:
                    return DataType.String;
                default:
                    return DataType.Undefined;
            }
        }
    }
}
