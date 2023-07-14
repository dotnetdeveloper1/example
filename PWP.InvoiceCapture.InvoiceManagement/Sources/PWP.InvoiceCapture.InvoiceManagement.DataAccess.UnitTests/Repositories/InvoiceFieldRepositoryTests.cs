using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class InvoiceFieldRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            context = (DatabaseContext)contextFactory.Create();
            target = new InvoiceFieldRepository(contextFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public void Instance_WhenDbContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceFieldRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceFieldIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync((InvoiceField)null, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceFieldsAreNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync((List<InvoiceField>)null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 1, 1, 1)]
        [DataRow(10, 2, 4, 2)]
        [DataRow(100, 3, 5, 5)]
        public async Task CreateAsync_WhenInvoiceFieldIsNotNull_ShouldSaveAndGenerateIdAndCreatedDateAndModifiedDate(int fieldId, int orderNumber, int groupId, int invoiceId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingInvoice = CreateInvoice(invoiceId);
            var existingField = CreateField(fieldId, groupId);

            context.FieldGroups.Add(existingFieldGroup);
            context.Invoices.Add(existingInvoice);
            context.Fields.Add(existingField);

            context.SaveChanges();

            var expectedInvoiceField = CreateInvoiceField(0, fieldId, invoiceId);

            await target.CreateAsync(expectedInvoiceField, cancellationToken);

            var actualInvoiceField = context.InvoiceFields.FirstOrDefault();

            Assert.IsNotNull(actualInvoiceField);
            Assert.AreNotEqual(0, actualInvoiceField.Id);
            Assert.AreNotEqual(default, actualInvoiceField.CreatedDate);
            Assert.AreNotEqual(default, actualInvoiceField.ModifiedDate);

            AssertInvoiceFieldsAreEqual(expectedInvoiceField, actualInvoiceField);
        }

        [TestMethod]
        [DataRow(1, 1, 1, 1)]
        [DataRow(10, 2, 4, 2)]
        [DataRow(100, 3, 5, 5)]
        public async Task CreateAsync_WhenInvoiceFieldsAreNotNull_ShouldSaveAndGenerateIdAndCreatedDateAndModifiedDate(int fieldId, int orderNumber, int groupId, int invoiceId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingInvoice = CreateInvoice(invoiceId);
            var existingField1 = CreateField(fieldId, groupId);
            var existingField2 = CreateField(fieldId + 1, groupId);

            context.FieldGroups.Add(existingFieldGroup);
            context.Invoices.Add(existingInvoice);
            context.Fields.Add(existingField1);
            context.Fields.Add(existingField2);

            context.SaveChanges();

            var expectedInvoiceFields = new List<InvoiceField> { CreateInvoiceField(0, fieldId, invoiceId), CreateInvoiceField(0, fieldId + 1, invoiceId) };

            await target.CreateAsync(expectedInvoiceFields, cancellationToken);

            var actualInvoiceFields = context.InvoiceFields;

            foreach (var actualInvoiceField in actualInvoiceFields)
            {
                Assert.IsNotNull(actualInvoiceField);
                Assert.AreNotEqual(0, actualInvoiceField.Id);
                Assert.AreNotEqual(default, actualInvoiceField.CreatedDate);
                Assert.AreNotEqual(default, actualInvoiceField.ModifiedDate);

                var expectedInvoiceField = expectedInvoiceFields.FirstOrDefault(invoiceField => invoiceField.FieldId == actualInvoiceField.FieldId);

                AssertInvoiceFieldsAreEqual(expectedInvoiceField, actualInvoiceField);
            }
        }

        [TestMethod]
        public async Task UpdateAsync_WhenInvoiceFieldIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.UpdateAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateAsync_WhenInvoiceFieldIdIsLessOrEqualsZero_ShouldThrowArgumentException(int invoiceFieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateAsync(invoiceFieldId, new InvoiceField(), cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 1, 2, 1)]
        [DataRow(10, 2, 3, 3)]
        [DataRow(100, 3, 4, 4)]
        public async Task UpdateAsync_WhenInvoiceFieldIsNotNull_ShouldUpdateFieldAndModifiedDate(int fieldId, int orderNumber, int groupId, int invoiceId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingInvoice = CreateInvoice(invoiceId);
            var existingField = CreateField(fieldId, groupId);
            var invoiceField = CreateInvoiceField(1, fieldId, invoiceId);

            context.Fields.Add(existingField);
            context.Invoices.Add(existingInvoice);
            context.FieldGroups.Add(existingFieldGroup);
            context.InvoiceFields.Add(invoiceField);

            context.SaveChanges();

            var updatedInvoiceField = CreateInvoiceField(1, fieldId, invoiceId);

            updatedInvoiceField.Value = "UpdatedValue";

            await target.UpdateAsync(invoiceField.Id, updatedInvoiceField, cancellationToken);

            var actualInvoiceField = context.InvoiceFields.FirstOrDefault();

            Assert.IsNotNull(actualInvoiceField);
            Assert.AreNotEqual(default, actualInvoiceField.ModifiedDate);

            AssertInvoiceFieldsAreEqual(updatedInvoiceField, actualInvoiceField);
        }

        [TestMethod]
        [DataRow(1, 1, 2, 1)]
        [DataRow(10, 2, 3, 3)]
        [DataRow(100, 3, 4, 4)]
        public async Task UpdateAsync_WhenInvoiceFieldsAreNotNull_ShouldUpdateFieldAndModifiedDate(int fieldId, int orderNumber, int groupId, int invoiceId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingInvoice = CreateInvoice(invoiceId);
            var existingField1 = CreateField(fieldId, groupId);
            var existingField2 = CreateField(fieldId + 1, groupId);            
            var invoiceField1 = CreateInvoiceField(1, existingField1.Id, invoiceId);
            var invoiceField2 = CreateInvoiceField(2, existingField2.Id, invoiceId);
            var initialInvoiceFields = new List<InvoiceField> { invoiceField1, invoiceField2 };

            context.Fields.Add(existingField1);
            context.Fields.Add(existingField2);
            context.Invoices.Add(existingInvoice);
            context.FieldGroups.Add(existingFieldGroup);
            context.InvoiceFields.AddRange(initialInvoiceFields);

            context.SaveChanges();

            var updatedInvoiceField1 = CreateInvoiceField(1, existingField1.Id, invoiceId);
            var updatedInvoiceField2 = CreateInvoiceField(2, existingField2.Id, invoiceId);

            var dateInFuture = DateTime.UtcNow.AddYears(10);
            updatedInvoiceField1.Value = "UpdatedValue1";
            updatedInvoiceField1.CreatedDate = dateInFuture;

            updatedInvoiceField2.Value = "UpdatedValue2";
            updatedInvoiceField2.CreatedDate = dateInFuture;

            var updatedInvoiceFields = new List<InvoiceField> { updatedInvoiceField1, updatedInvoiceField2 };

            await target.UpdateAsync(updatedInvoiceFields, cancellationToken);

            foreach (var expectedInvoiceField in updatedInvoiceFields)
            {
                var actualInvoiceField = context.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.FieldId == expectedInvoiceField.FieldId);
                var initialInviceField = initialInvoiceFields.First(initialInvoiceField => initialInvoiceField.FieldId == expectedInvoiceField.FieldId);

                Assert.IsNotNull(actualInvoiceField);
                Assert.AreEqual(initialInviceField.CreatedDate, actualInvoiceField.CreatedDate);

                AssertInvoiceFieldsAreEqual(actualInvoiceField, actualInvoiceField);
            }
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetListAsync_WhenInvoiceFieldsCollectionIsNotEmpty_ShouldReturnList(int count)
        {
            var invoiceFields = Enumerable
                .Range(1, count)
                .Select(index => CreateInvoiceField(index, 0, 0))
                .ToList();

            context.InvoiceFields.AddRange(invoiceFields);
            context.SaveChanges();

            var actualInvoicePages = await target.GetListAsync(123123, cancellationToken);

            Assert.IsNotNull(actualInvoicePages);
            Assert.AreEqual(0, actualInvoicePages.Count);
        }

        [TestMethod]
        [DataRow(10, 2, 3)]
        [DataRow(1, 5, 7)]
        public async Task GetListAsync_WhenInvoiceFieldsCollectionContainsDeletedFieldOrGroup_ShouldReturnListWithoutDeletedFieldsAndGroups(int invociceId, int groupId, int fieldId)
        {
            var fieldGroup1 = CreateFieldGroup(groupId, 1);
            var fieldGroup2 = CreateFieldGroup(groupId + 1, 2);
            var fieldGroup3 = CreateFieldGroup(groupId + 2, 2);
            var field1 = CreateField(fieldId, fieldGroup1.Id);
            var field2 = CreateField(fieldId + 1, fieldGroup2.Id);
            var field3 = CreateField(fieldId + 2, fieldGroup3.Id);

            context.FieldGroups.Add(fieldGroup1);
            context.FieldGroups.Add(fieldGroup2);
            context.FieldGroups.Add(fieldGroup3);
            context.Fields.Add(field1);
            context.Fields.Add(field2);
            context.Fields.Add(field3);

            InvoiceField[] invoiceFields = { 
                CreateInvoiceField(1, field1.Id, invociceId), 
                CreateInvoiceField(2, field2.Id, invociceId), 
                CreateInvoiceField(3, field3.Id, invociceId) 
            } ;

            context.InvoiceFields.AddRange(invoiceFields);
            context.SaveChanges();

            var actualInvoiceFields = await target.GetListAsync(invociceId, cancellationToken);

            Assert.IsNotNull(actualInvoiceFields);
            Assert.AreEqual(3, actualInvoiceFields.Count);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task DeleteAsync_WhenIdIsCorrect_ShouldDeleteInvoiceField(int count)
        {
            var invoiceFields = Enumerable
                .Range(1, count)
                .Select(index => CreateInvoiceField(index, 0, 0))
                .ToList();

            context.InvoiceFields.AddRange(invoiceFields);
            context.SaveChanges();

            await target.DeleteAsync(invoiceFields[0].Id, cancellationToken);

            var deletedInvoice = context.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Id == invoiceFields[0].Id);
            Assert.IsNull(deletedInvoice);
            Assert.AreEqual(invoiceFields.Count() - 1, context.InvoiceFields.Count());
        }

        [TestMethod]
        [DataRow(10)]
        public async Task DeleteAsync_WhenIdsAreCorrect_ShouldDeleteAllInvoiceFields(int count)
        {
            var invoiceFields = Enumerable
                .Range(1, count)
                .Select(index => CreateInvoiceField(index, 0, 0))
                .ToList();

            context.InvoiceFields.AddRange(invoiceFields);
            context.SaveChanges();

            await target.DeleteAsync(invoiceFields.Select(invoiceField => invoiceField.Id).ToList(), cancellationToken);

            Assert.IsFalse(context.InvoiceFields.Any());
        }

        [TestMethod]
        [DataRow(10, 1, 2)]
        [DataRow(5, 2, 3)]
        public async Task DeleteAsync_WhenInvoiceFieldIdIsCorrect_ShouldInvoiceField(int count, int invoiceId, int fieldId)
        {
            var invoiceFields = Enumerable
                .Range(1, count)
                .Select(index => CreateInvoiceField(index, fieldId, invoiceId))
                .ToList();

            context.InvoiceFields.AddRange(invoiceFields);
            context.SaveChanges();

            await target.DeleteAsync(invoiceFields[0].Id, cancellationToken);

            var deletedInvoice = context.InvoiceFields.FirstOrDefault(invoiceField => invoiceField.Id == invoiceFields[0].Id);
            Assert.IsNull(deletedInvoice);
            Assert.AreEqual(invoiceFields.Count() - 1, context.InvoiceFields.Count());
        }

        [TestMethod]
        [DataRow(10, 1, 2)]
        [DataRow(5, 2, 3)]
        public async Task DeleteAllInvoiceFieldAsync_WhenInvoiceIdIsCorrect_ShouldAllInvoiceFields(int count, int invoiceId, int fieldId)
        {
            var invoiceFields = Enumerable
                .Range(1, count)
                .Select(index => CreateInvoiceField(index, fieldId, invoiceId))
                .ToList();

            context.InvoiceFields.AddRange(invoiceFields);
            context.SaveChanges();

            await target.DeleteByInvoiceIdAsync(invoiceId, cancellationToken);

            Assert.AreEqual(context.InvoiceFields.Count(), 0);
        }

        private void AssertInvoiceFieldsAreEqual(InvoiceField expected, InvoiceField actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Value, actual.Value);
            Assert.AreEqual(expected.FieldId, actual.FieldId);
            Assert.AreEqual(expected.InvoiceId, actual.InvoiceId);

            // Ensure all properties are tested
            Assert.AreEqual(7, actual.GetType().GetProperties().Length);
        }

        private FieldGroup CreateFieldGroup(int id, int orderNumber)
        {
            return new FieldGroup
            {
                Id = id,
                DisplayName = "DisplayName",
                OrderNumber = orderNumber
            };
        }

        private InvoiceField CreateInvoiceField(int id, int fieldId, int invoiceId)
        {
            return new InvoiceField
            {
                Id = id,
                Value = $"Value{id}",
                FieldId = fieldId,
                InvoiceId = invoiceId
            };
        }

        private Field CreateField(int id, int fieldGroupId)
        {
            return new Field
            {
                Id = id,
                GroupId = fieldGroupId,
                OrderNumber = 1,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                DefaultValue = $"DefaultValue{id}",
                DisplayName = $"DisplayName{id}",
                IsProtected = false,
                IsRequired = true,
                MaxLength = 10,
                MinLength = 1,
                MaxValue = 50,
                MinValue = 0,
                Type = FieldType.Decimal
            };
        }

        private Invoice CreateInvoice(int id, InvoiceState state = InvoiceState.Active)
        {
            var random = new Random(id);

            return new Invoice
            {
                Id = id,
                Name = $"Name{id}",
                FileId = $"File{id}",
                FileName = $"FileName{id}",
                FileSourceType = FileSourceType.API,
                Status = InvoiceStatus.NotStarted,
                InvoiceState = state
            };
        }

        private DatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IInvoiceFieldRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
