using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Mappers
{
    internal class AnnotationMapper : IAnnotationMapper
    {
        public InvoiceField ToInvoiceField(int invoiceId, Annotation annotation, List<Field> fields, CultureInfo cultureInfo)
        {
            if (annotation == null || invoiceId <= 0)
            {
                return null;
            }

            int.TryParse(annotation.FieldType, out int fieldId);
            var type = fields.Single(field => field.Id == fieldId).Type;

            return new InvoiceField() 
            { 
                Value = CorrectAnnotationStringRepresenation(annotation.FieldValue, type, cultureInfo), 
                InvoiceId = invoiceId,
                FieldId = fieldId
            };
        }

        private string CorrectAnnotationStringRepresenation(string fieldValue, FieldType fieldType, CultureInfo cultureInfo)
        {
            if (fieldType == FieldType.DateTime)
            {
                return DateTime.Parse(fieldValue, cultureInfo).ToString();
            }

            return fieldValue;
        }

        public InvoiceLine ToInvoiceLine(LineAnnotation lineAnnotation, int invoiceId, CultureInfo cultureInfo)
        {
            if (lineAnnotation?.LineItemAnnotations == null)
            {
                return null;
            }

            var totalAmount = lineAnnotation.LineItemAnnotations.FirstOrDefault(item => item.FieldType == InvoiceLineFieldTypes.Total);
            var invoiceNumber = lineAnnotation.LineItemAnnotations.FirstOrDefault(item => item.FieldType == InvoiceLineFieldTypes.Number);
            var description = lineAnnotation.LineItemAnnotations.FirstOrDefault(item => item.FieldType == InvoiceLineFieldTypes.Description);
            var price = lineAnnotation.LineItemAnnotations.FirstOrDefault(item => item.FieldType == InvoiceLineFieldTypes.Price);
            var quantity = lineAnnotation.LineItemAnnotations.FirstOrDefault(item => item.FieldType == InvoiceLineFieldTypes.Quantity);

            return new InvoiceLine()
            {
                InvoiceId = invoiceId,
                OrderNumber = lineAnnotation.OrderNumber,
                Total = totalAmount?.FieldValue.ToNullableDecimal(invariantCulture) ?? 0,
                Number = invoiceNumber?.FieldValue,
                Description = description?.FieldValue,
                Price = price?.FieldValue.ToNullableDecimal(invariantCulture),
                Quantity = quantity?.FieldValue.FromHourOrDecimalToDecimal(invariantCulture)
            };
        }
        
        private readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;
    }
}
