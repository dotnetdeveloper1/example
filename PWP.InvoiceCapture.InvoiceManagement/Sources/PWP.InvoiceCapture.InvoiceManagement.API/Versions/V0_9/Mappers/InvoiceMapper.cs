using PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InvoiceV1_0 = PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models.Invoice;
using InvoiceV0_9= PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Models.Invoice;

namespace PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Mappers
{
    public class InvoiceMapper
    {
        public static List<InvoiceV0_9> ToV0_9Models(List<InvoiceV1_0> invoices)
        {
           return invoices.Select(invoice => ToV0_9Model(invoice)).ToList();
        }

        public static InvoiceV0_9 ToV0_9Model(InvoiceV1_0 invoice)
        {
            if (invoice == null)
            {
                return null;
            }

            var result = new InvoiceV0_9()
            {
                Id = invoice.Id,
                Name = invoice.Name,
                FileName = invoice.FileName,
                FileSourceType = invoice.FileSourceType,
                FileId = invoice.FileId,
                Status = invoice.Status,
                InvoiceState = invoice.InvoiceState,
                FromEmailAddress = invoice.FromEmailAddress,
                ValidationMessage = invoice.ValidationMessage,
                CreatedDate = invoice.CreatedDate,
                ModifiedDate = invoice.ModifiedDate,
                InvoiceLines = invoice.InvoiceLines
            };

            if (invoice.InvoiceFields != null)
            {
                result.InvoiceDate = ParseDate(invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.InvoiceDate));
                result.DueDate = ParseDate(invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.DueDate));
                result.SubTotal = ParseDecimal(invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.SubTotal));
                result.Total = ParseDecimal(invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.Total));
                result.FreightAmount = ParseDecimal(invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.FreightAmount));
                result.TaxAmount = ParseDecimal(invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.TaxAmount));
                result.TaxNumber = invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.TaxNumber)?.Value;
                result.InvoiceNumber = invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.InvoiceNumber)?.Value;
                result.PONumber = invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.PoNumber)?.Value;
                result.Contacts = new List<Contact>() { CreateContact(invoice) };
            }

            return result;
        }

        private static DateTime? ParseDate(InvoiceField invoiceField)
        {
            if (invoiceField == null || invoiceField.Value == null)
            {
                return null;
            }
            return DateTime.Parse(invoiceField.Value);
        }

        private static decimal? ParseDecimal(InvoiceField invoiceField)
        {
            if (invoiceField == null || invoiceField.Value == null)
            {
                return null;
            }
            return Decimal.Parse(invoiceField.Value);
        }

        private static Contact CreateContact(InvoiceV1_0 invoice)
        {
            var vendorName = invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.VendorName);
            var result = new Contact()
            {
                ContactType = ContactType.Vendor,
                Name = vendorName?.Value,
                Email = invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.VendorEmail)?.Value,
                Address = invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.VendorAddress)?.Value,
                Website = invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.VendorWebsite)?.Value,
                Phone = invoice.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Field.Id == FieldTypes.VendorPhone)?.Value,
                InvoiceId = invoice.Id
            };

            if (vendorName != null)
            {
                result.CreatedDate = vendorName.CreatedDate;
                result.ModifiedDate = vendorName.ModifiedDate;
            }

            return result;
        }
    }
}
