using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Mappers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Mappers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AnnotationMapperTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new AnnotationMapper();
        }
       
        [TestMethod]
        [DataRow(-10)]
        [DataRow(1)]
        [DataRow(10)]
        public void ToInvoiceLine_WhenLineAnnotationIsNull_ShouldReturnNull(int invoiceId)
        {
            var invoiceLine = target.ToInvoiceLine(null, invoiceId, cultureInfo);

            Assert.IsNull(invoiceLine);
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(1)]
        [DataRow(10)]
        public void ToInvoiceLine_WhenLineItemAnnotationsIsNull_ShouldReturnNull(int invoiceId)
        {
            var invoiceLine = target.ToInvoiceLine(new LineAnnotation(), invoiceId, cultureInfo);

            Assert.IsNull(invoiceLine);
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 2)]
        public void ToInvoiceLine_WhenLineAnnotationsIsNotNull_ShouldReturnInvoiceLine(int invoiceId, int orderNumber)
        {
            var lineAnnotation = new LineAnnotation()
            {
                LineItemAnnotations = new List<Annotation>
                {
                    new Annotation { FieldType = InvoiceLineFieldTypes.Description, FieldValue = description },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Number, FieldValue = number },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Price, FieldValue = price.ToString() },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Quantity, FieldValue = quantity.ToString() },
                    new Annotation { FieldType = InvoiceLineFieldTypes.Total, FieldValue = total.ToString() }
                },
                OrderNumber = orderNumber
            };

            var invoiceLine = target.ToInvoiceLine(lineAnnotation, invoiceId, cultureInfo);

            Assert.IsNotNull(invoiceLine);
            Assert.AreEqual(invoiceId, invoiceLine.InvoiceId);
            Assert.AreEqual(invoiceLine.OrderNumber, orderNumber);
            Assert.AreEqual(invoiceLine.Number, number);
            Assert.AreEqual(invoiceLine.Quantity, quantity);
            Assert.AreEqual(invoiceLine.Total, total);
            Assert.AreEqual(invoiceLine.Price, price);
            Assert.AreEqual(invoiceLine.Description, description);
            Assert.AreEqual(default, invoiceLine.CreatedDate);
            Assert.AreEqual(default, invoiceLine.ModifiedDate);

            // Ensure all properties are tested
            Assert.AreEqual(10, invoiceLine.GetType().GetProperties().Length);
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 2)]
        public void ToInvoiceLine_WhenAnnotationsContainsRequiredFieldsOnly_ShouldReturnInvoiceLine(int invoiceId, int orderNumber)
        {
            var lineAnnotation = new LineAnnotation()
            {
                LineItemAnnotations = new List<Annotation>
                {
                    new Annotation { FieldType = InvoiceLineFieldTypes.Total, FieldValue = total.ToString() }
                },
                OrderNumber = orderNumber
            };

            var invoiceLine = target.ToInvoiceLine(lineAnnotation, invoiceId, cultureInfo);

            Assert.IsNotNull(invoiceLine);
            Assert.AreEqual(invoiceId, invoiceLine.InvoiceId);
            Assert.AreEqual(invoiceLine.OrderNumber, orderNumber);
            Assert.AreEqual(invoiceLine.Total, total);
            Assert.AreEqual(invoiceLine.Number, null);
            Assert.AreEqual(invoiceLine.Quantity, null);
            Assert.AreEqual(invoiceLine.Price, null);
            Assert.AreEqual(invoiceLine.Description, null);
            Assert.AreEqual(default, invoiceLine.CreatedDate);
            Assert.AreEqual(default, invoiceLine.ModifiedDate);

            // Ensure all properties are tested
            Assert.AreEqual(10, invoiceLine.GetType().GetProperties().Length);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void ToInvoiceField_WhenAnnotationIsNull_ShouldReturnNull(int invoiceId)
        {

            var invoiceField =  target.ToInvoiceField(invoiceId, null, allInvoiceFieldsList, cultureInfo);

            Assert.IsNull(invoiceField);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public void ToInvoiceField_InvoiceIdIsLessOREqualsToZero_ShouldReturnNull(int invoiceId)
        {
            var invoiceField = target.ToInvoiceField(invoiceId, allAnnotationsList[0], allInvoiceFieldsList, cultureInfo);

            Assert.IsNull(invoiceField);
        }

        [TestMethod]
        public void ToInvoiceField_WhenFiedIsDate_ShouldReturnDate()
        {
            var invoiceField = target.ToInvoiceField(1, allAnnotationsList[6], allInvoiceFieldsList, new CultureInfo("en-CA"));

            Assert.IsNotNull(invoiceField);
            Assert.AreEqual("10/20/2020 12:00:00 AM", invoiceField.Value);
            Assert.IsTrue(DateTime.TryParse(invoiceField.Value, out DateTime date));
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 1)]
        public void ToInvoiceField_WhenAnnotationExists_ShouldReturnInvoiceField(int invoiceId, int annotationIndex)
        {
            var annotation = allAnnotationsList[annotationIndex];

            var invoiceField = target.ToInvoiceField(invoiceId, annotation, allInvoiceFieldsList, cultureInfo);

            Assert.IsNotNull(invoiceField);
            Assert.AreEqual(annotation.FieldValue, invoiceField.Value);
            Assert.AreEqual(annotation.FieldType, invoiceField.FieldId.ToString());
            Assert.AreEqual(invoiceId, invoiceField.InvoiceId);
        }

        private readonly List<Annotation> allAnnotationsList = new List<Annotation>()
        {
            new Annotation { FieldType = FieldTypes.VendorName.ToString(), FieldValue = "name" },
            new Annotation { FieldType = FieldTypes.VendorAddress.ToString(), FieldValue = "address" },
            new Annotation { FieldType = FieldTypes.TaxNumber.ToString(), FieldValue = "taxN" },
            new Annotation { FieldType = FieldTypes.VendorPhone.ToString(), FieldValue = "phone" },
            new Annotation { FieldType = FieldTypes.VendorEmail.ToString(), FieldValue = "email" },
            new Annotation { FieldType = FieldTypes.VendorWebsite.ToString(), FieldValue = "website" },
            new Annotation { FieldType = FieldTypes.InvoiceDate.ToString(), FieldValue = "2020/10/20" },
            new Annotation { FieldType = FieldTypes.DueDate.ToString(), FieldValue = "2020-01-02" },
            new Annotation { FieldType = FieldTypes.PoNumber.ToString(), FieldValue = "poN" },
            new Annotation { FieldType = FieldTypes.InvoiceNumber.ToString(), FieldValue = "invN1" },
            new Annotation { FieldType = FieldTypes.TaxAmount.ToString(), FieldValue = "1.2" },
            new Annotation { FieldType = FieldTypes.FreightAmount.ToString(), FieldValue = "1.1" },
            new Annotation { FieldType = FieldTypes.SubTotal.ToString(), FieldValue = "2" },
            new Annotation { FieldType = FieldTypes.Total.ToString(), FieldValue = "4.3" }
        };

        private readonly List<Field> allInvoiceFieldsList = new List<Field>()
        {
            new Field(){Id = FieldTypes.VendorName},
            new Field(){Id = FieldTypes.VendorAddress},
            new Field(){Id = FieldTypes.TaxNumber},
            new Field(){Id = FieldTypes.VendorPhone},
            new Field(){Id = FieldTypes.VendorEmail},
            new Field(){Id = FieldTypes.VendorWebsite},
            new Field(){Id = FieldTypes.InvoiceDate, Type = Contract.Enumerations.FieldType.DateTime},
            new Field(){Id = FieldTypes.PoNumber},
            new Field(){Id = FieldTypes.TaxAmount},
            new Field(){Id = FieldTypes.FreightAmount},
            new Field(){Id = FieldTypes.SubTotal},
            new Field(){Id = FieldTypes.Total}
        };

        private AnnotationMapper target;
        private const string description = "Description";
        private const string number = "Number";
        private const decimal price = 10.1M;
        private const int quantity = 10;
        private const int total = 15;
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        private readonly CancellationToken cancelationToken = CancellationToken.None;
    }
}
