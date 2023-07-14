using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Enumerations;
using PWP.InvoiceCapture.OCR.Recognition.Business.Mapper;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Mappers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FieldMapperTests
    {
        [TestMethod]
        public void ToFieldTargetField_WhenFieldIsNull_ShouldReturnNull()
        {
            var fieldTargetField = target.ToFieldTargetField(null);

            Assert.IsNull(fieldTargetField);
        }

        [TestMethod]
        public void ToFieldTargetFieldList_WhenFieldsAreNull_ShouldReturnNull()
        {
            var fieldTargetFields = target.ToFieldTargetFieldList(null);

            Assert.IsNull(fieldTargetFields);
        }

        [TestMethod]
        public void ToFieldTargetField_WhenTargetFieldTypeIsNull_ShouldReturnUnknownFormRecognizerFieldType()
        {
            var field = CreateField(FieldType.DateTime, null);

            var fieldTargetFieldType = target.ToFieldTargetField(field);

            Assert.IsNotNull(fieldTargetFieldType);
            Assert.AreEqual(fieldTargetFieldType.FormRecognizerFieldType, FormRecognizerFieldType.Unknown);
        }

        [TestMethod]
        [DataRow(FieldType.DateTime, TargetFieldType.CustomerId)]
        [DataRow(FieldType.Decimal, TargetFieldType.InvoiceDate)]
        [DataRow(FieldType.String, TargetFieldType.ServiceEndDate)]
        public void ToFieldTargetField_WhenFieldIsNotNull_ShouldReturnFieldTargetField(FieldType fieldType, TargetFieldType targetFieldType)
        {
            var field = CreateField(fieldType, targetFieldType);

            var fieldTargetField = target.ToFieldTargetField(field);

            Assert.IsNotNull(fieldTargetField);
            Assert.AreEqual(fieldTargetField.FieldId, field.Id);
            Assert.AreEqual(fieldTargetField.FieldName, field.DisplayName);
            Assert.AreEqual(GetOcrDataType(field.Type), fieldTargetField.DataType);
            Assert.AreEqual((int)field.TargetFieldType, (int)fieldTargetField.FormRecognizerFieldType);
        }

        [TestMethod]
        [DataRow(FieldType.DateTime, TargetFieldType.CustomerId)]
        [DataRow(FieldType.Decimal, TargetFieldType.InvoiceDate)]
        [DataRow(FieldType.String, TargetFieldType.ServiceEndDate)]
        public void ToFieldTargetFieldList_WhenFieldsAreNotNull_ShouldReturnFieldTargetFieldList(FieldType fieldType, TargetFieldType targetFieldType)
        {
            var fields = new List<Field>() { CreateField(fieldType, targetFieldType), CreateField(fieldType, targetFieldType) };

            var fieldTargetFieldList = target.ToFieldTargetFieldList(fields);

            foreach (var fieldTargetField in fieldTargetFieldList)
            {
                var field = fields.First(actualField => actualField.Id == fieldTargetField.FieldId);

                Assert.IsNotNull(fieldTargetField);
                Assert.AreEqual(fieldTargetField.FieldName, field.DisplayName);
                Assert.AreEqual(GetOcrDataType(field.Type), fieldTargetField.DataType);
                Assert.AreEqual((int)field.TargetFieldType, (int)fieldTargetField.FormRecognizerFieldType);
            }
        }

        private DataType GetOcrDataType(FieldType fieldType)
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

        private Field CreateField(FieldType fieldType, TargetFieldType? targetFieldType)
        {
            return new Field()
            {
                Id = 1,
                Type = fieldType,
                TargetFieldType = targetFieldType,
                DisplayName = "Name"
            };
        }

        private readonly FieldMapper target = new FieldMapper();
    }
}
