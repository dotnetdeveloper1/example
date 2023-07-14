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
    public class FieldRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            context = (DatabaseContext)contextFactory.Create();
            target = new FieldRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new FieldRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenFieldIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(10, 2, 4)]
        [DataRow(100, 3, 5)]
        public async Task CreateAsync_WhenFieldIsNotNull_ShouldSaveAndGenerateIdAndCreatedDateAndModifiedDate(int fieldId, int orderNumber, int groupId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var fieldGroupId = existingFieldGroup.Id;

            context.FieldGroups.Add(existingFieldGroup);
            context.SaveChanges();

            var expectedField = CreateField(fieldId, fieldGroupId);

            await target.CreateAsync(expectedField, cancellationToken);

            var actualField = context.Fields.FirstOrDefault();

            Assert.IsNotNull(actualField);
            Assert.AreNotEqual(0, actualField.Id);
            Assert.AreNotEqual(default, actualField.CreatedDate);
            Assert.AreNotEqual(default, actualField.ModifiedDate);

            AssertFieldsAreEqual(expectedField, actualField);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenFieldIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.UpdateAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateAsync_WhenFieldIdIsLessOrEqualsZero_ShouldThrowArgumentException(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateAsync(fieldId, new Field(), cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 1, 2)]
        [DataRow(10, 2, 3)]
        [DataRow(100, 3, 4)]
        public async Task UpdateAsync_WhenFieldIsNotNull_ShouldUpdateFieldAndModifiedDate(int fieldId, int orderNumber, int groupId)
        {
            var fieldGroup = CreateFieldGroup(groupId, orderNumber);
            var expectedField = CreateField(fieldId, fieldGroup.Id);

            context.FieldGroups.Add(fieldGroup);
            context.Fields.Add(expectedField);
            context.SaveChanges();

            var updatedField = CreateField(groupId, orderNumber + 1);
            updatedField.Id = fieldId;
            updatedField.DisplayName = Guid.NewGuid().ToString();
            updatedField.IsRequired = !updatedField.IsRequired;
            updatedField.TargetFieldType = TargetFieldType.CustomerId;

            await target.UpdateAsync(expectedField.Id, updatedField, cancellationToken);

            var actualField = context.Fields.FirstOrDefault();

            Assert.IsNotNull(actualField);
            Assert.AreNotEqual(default, actualField.ModifiedDate);

            AssertFieldsAreEqual(updatedField, actualField);
        }

        [TestMethod]
        public async Task GetListAsync_WhenFieldCollectionIsEmpty_ShouldReturnEmptyList()
        {
            var actualFieldGroups = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualFieldGroups);
            Assert.AreEqual(0, actualFieldGroups.Count);
        }

        [TestMethod]
        [DataRow(2, 1)]
        [DataRow(3, 10)]
        public async Task GetAsync_WhenFieldWithIdExists_ShouldReturnValidObject(int fieldId, int fieldGroupId)
        {
            var expectedFieldGroup = CreateFieldGroup(fieldGroupId, 1);
            var expectedField = CreateField(fieldId, fieldGroupId);

            context.FieldGroups.Add(expectedFieldGroup);
            context.Fields.Add(expectedField);
            context.SaveChanges();

            var actualField = await target.GetAsync(fieldId, cancellationToken);

            Assert.IsNotNull(actualField);

            AssertFieldsAreEqual(expectedField, actualField);
        }

        [TestMethod]
        [DataRow(2, 1)]
        [DataRow(3, 10)]
        public async Task GetAsync_WhenFieldWasDeleted_ShouldReturnNull(int fieldId, int fieldGroupId)
        {
            var expectedFieldGroup = CreateFieldGroup(fieldGroupId, 1);

            var expectedField = CreateField(fieldId, fieldGroupId);
            expectedField.IsDeleted = true;

            context.FieldGroups.Add(expectedFieldGroup);
            context.Fields.Add(expectedField);
            context.SaveChanges();

            var actualField = await target.GetAsync(fieldId, cancellationToken);

            Assert.IsNull(actualField);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteByFieldAsync_WhenFieldIdIsZeroOrLessThenZero_ShouldReturnArgumentException(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(fieldId, cancellationToken));
        }

        [TestMethod]
        [DataRow(2, 1)]
        [DataRow(3, 10)]
        public async Task DeleteAsync_WhenFieldExists_ShouldMarkAsDeleted(int fieldId, int fieldGroupId)
        {
            var expectedFieldGroup = CreateFieldGroup(fieldGroupId, 1);
            var expectedField = CreateField(fieldId, fieldGroupId);

            context.FieldGroups.Add(expectedFieldGroup);
            context.Fields.Add(expectedField);
            context.SaveChanges();

            await target.DeleteAsync(fieldId, cancellationToken);

            var deletedField = context.Fields.FirstOrDefault(field => field.Id == fieldId);

            Assert.IsNotNull(deletedField);
            Assert.IsTrue(deletedField.IsDeleted);
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(-1, 34)]
        public async Task DeleteByFieldIdsAsync_WhenFieldIdIsZeroOrLessThenZero_ShouldReturnArgumentException(int id1, int id2)
        {
            var fieldIds = new List<int>() { id1, id2 };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(fieldIds, cancellationToken));
        }

        [TestMethod]
        public async Task DeleteByFieldIdsAsync_WhenListIsNull_ShouldReturnArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.DeleteAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task DeleteByIdsListAsync_WhenFieldsExists_ShouldMarkAsDeleted(int fieldGroupId)
        {
            var expectedFieldGroup = CreateFieldGroup(fieldGroupId, 1);
            var expectedFields = Enumerable.Range(1, 10)
                .Select(fieldId => CreateField(fieldId, fieldGroupId))
                .ToList();

            context.FieldGroups.Add(expectedFieldGroup);
            context.Fields.AddRange(expectedFields);
            context.SaveChanges();

            var fieldIds = expectedFields.Select(field => field.Id).ToList();

            await target.DeleteAsync(fieldIds, cancellationToken);

            Assert.IsTrue(context.Fields.All(field => field.IsDeleted));
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

        private Field CreateField(int id, int groupId)
        {
            return new Field
            {
                Id = id,
                GroupId = groupId,
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
                Type = FieldType.Decimal,
                Formula = "[1]+[2]"
            };
        }

        private bool AssertFieldsAreEqual(Field expected, Field actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.IsProtected, actual.IsProtected);
            Assert.AreEqual(expected.IsDeleted, actual.IsDeleted);
            Assert.AreEqual(expected.DisplayName, actual.DisplayName);
            Assert.AreEqual(expected.OrderNumber, actual.OrderNumber);
            Assert.AreEqual(expected.DefaultValue, actual.DefaultValue);
            Assert.AreEqual(expected.GroupId, actual.GroupId);
            Assert.AreEqual(expected.IsProtected, actual.IsProtected);
            Assert.AreEqual(expected.IsRequired, actual.IsRequired);
            Assert.AreEqual(expected.MaxLength, actual.MaxLength);
            Assert.AreEqual(expected.MaxValue, actual.MaxValue);
            Assert.AreEqual(expected.MinLength, actual.MinLength);
            Assert.AreEqual(expected.MinValue, actual.MinValue);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.MinLength, actual.MinLength);
            Assert.AreEqual(expected.TargetFieldType, actual.TargetFieldType);
            Assert.AreEqual(expected.Formula, actual.Formula);

            // Ensure all properties are tested
            Assert.AreEqual(17, actual.GetType().GetProperties().Length);

            return true;
        }

        private void AssertFieldGroupsAreEqual(FieldGroup expected, FieldGroup actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.DisplayName, actual.DisplayName);
            Assert.AreEqual(expected.OrderNumber, actual.OrderNumber);
            Assert.AreEqual(expected.IsDeleted, actual.IsDeleted);

            // Ensure all properties are tested
            Assert.AreEqual(8, actual.GetType().GetProperties().Length);
        }

        private DatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IFieldRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
