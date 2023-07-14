using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]    
    public class FieldGroudRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            context = (DatabaseContext)contextFactory.Create();
            target = new FieldGroupRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new FieldGroupRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenFieldGroudIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(10, 1)]
        [DataRow(100, 2)]
        public async Task CreateAsync_WhenFieldGroupIsNotNull_ShouldSaveAndGenerateIdAndCreatedDateAndModifiedDate(int seed, int orderNumber)
        {
            var fieldGroup = CreateFieldGroup(seed, orderNumber);
            fieldGroup.Id = 0;

            await target.CreateAsync(fieldGroup, cancellationToken);

            var actualFieldGroup = context.FieldGroups.FirstOrDefault();

            Assert.IsNotNull(actualFieldGroup);
            Assert.AreNotEqual(0, actualFieldGroup.Id);
            Assert.AreNotEqual(default, actualFieldGroup.CreatedDate);
            Assert.AreNotEqual(default, actualFieldGroup.ModifiedDate);

            AssertFieldGroupsAreEqual(fieldGroup, actualFieldGroup);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenFieldGroupIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.UpdateAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateAsync_WhenFieldGroupIdIsLessOrEqualsZero_ShouldThrowArgumentException(int fieldGroupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateAsync(fieldGroupId, new FieldGroup(), cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 1, "UpdatedName1", 2)]
        [DataRow(10, 2, "UpdatedName2", 3)]
        [DataRow(100, 3, "UpdatedName3", 4)]
        public async Task UpdateAsync_WhenFieldGroupIsNotNull_ShouldUpdateInvoiceAndModifiedDate(int seed, int orderNumber, string newName, int newOrderNumber)
        {
            var existingFieldGroup = CreateFieldGroup(seed, orderNumber);
            var fieldGroupId = existingFieldGroup.Id;

            context.FieldGroups.Add(existingFieldGroup);
            context.SaveChanges();

            var expectedFieldGroup = CreateFieldGroup(seed + 1, orderNumber);
            expectedFieldGroup.Id = fieldGroupId;
            expectedFieldGroup.OrderNumber = newOrderNumber;
            expectedFieldGroup.DisplayName = newName;

            await target.UpdateAsync(fieldGroupId, expectedFieldGroup, cancellationToken);

            var actualFieldGroup = context.FieldGroups.FirstOrDefault();

            Assert.IsNotNull(actualFieldGroup);
            Assert.AreNotEqual(default, actualFieldGroup.ModifiedDate);
            AssertFieldGroupsAreEqual(expectedFieldGroup, actualFieldGroup);
        }

        [TestMethod]
        public async Task GetListAsync_WhenFieldGroupCollectionIsEmpty_ShouldReturnEmptyList()
        {
            var actualFieldGroups = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualFieldGroups);
            Assert.AreEqual(0, actualFieldGroups.Count);
        }

        [TestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(5, 6, 7)]
        public async Task GetListAsync_WhenFieldGroupCollectionIsNotEmpty_ShouldReturnAll(int groupId, int fieldId, int groupOrderNumber)
        {
            var fieldGroup = CreateFieldGroup(groupId, groupOrderNumber);
            var field = CreateField(fieldId, groupId);

            context.FieldGroups.Add(fieldGroup);
            context.Fields.Add(field);
            await context.SaveChangesAsync();

            var actualFieldGroups = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualFieldGroups);
            Assert.AreEqual(1, actualFieldGroups.Count);
            Assert.AreEqual(1, actualFieldGroups[0].Fields.Count);

            AssertFieldGroupsAreEqual(fieldGroup, actualFieldGroups[0]);
            AssertFieldsAreEqual(field, actualFieldGroups[0].Fields[0]);
        }

        [TestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(5, 6, 7)]
        public async Task GetListAsync_WhenFieldGroupContainsDeletedGroup_ShouldReturnNotDeleted(int groupId, int fieldId, int groupOrderNumber)
        {
            var fieldGroup = CreateFieldGroup(groupId, groupOrderNumber);

            var deletedFieldGroup = CreateFieldGroup(groupId + 1, groupOrderNumber);
            deletedFieldGroup.IsDeleted = true;

            var field = CreateField(fieldId, groupId);

            context.FieldGroups.Add(fieldGroup);
            context.FieldGroups.Add(deletedFieldGroup);
            context.Fields.Add(field);
            await context.SaveChangesAsync();

            var actualFieldGroups = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualFieldGroups);
            Assert.AreEqual(1, actualFieldGroups.Count);
            Assert.AreEqual(1, actualFieldGroups[0].Fields.Count);

            AssertFieldGroupsAreEqual(fieldGroup, actualFieldGroups[0]);
            AssertFieldsAreEqual(field, actualFieldGroups[0].Fields[0]);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task GetAsync_WhenFieldGroupWitIdNotFound_ShouldReturnNull(int fieldGroupId)
        {
            context.FieldGroups.Add(CreateFieldGroup(fieldGroupId, 1));
            context.SaveChanges();

            var actualFieldGroup = await target.GetAsync(fieldGroupId + 1, cancellationToken);

            Assert.IsNull(actualFieldGroup);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task GetAsync_WhenFieldGroupIsDeleted_ShouldReturnNull(int fieldGroupId)
        {
            var fieldGroup = CreateFieldGroup(fieldGroupId, 1);
            fieldGroup.IsDeleted = true;

            context.FieldGroups.Add(fieldGroup);
            context.SaveChanges();

            var actualFieldGroup = await target.GetAsync(fieldGroupId + 1, cancellationToken);

            Assert.IsNull(actualFieldGroup);
        }

        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(10, 3)]
        public async Task GetAsync_WhenFieldGroupWithIdExists_ShouldReturnValidObject(int fieldGroupId, int fieldsCount)
        {
            var expectedFieldGroup = CreateFieldGroup(fieldGroupId, 1);
            var expectedFields = Enumerable.Range(1, fieldsCount)
                                .Select(index => CreateField(index, fieldGroupId))
                                .ToList();

            context.FieldGroups.Add(expectedFieldGroup);
            context.Fields.AddRange(expectedFields);
            context.SaveChanges();

            var actualFieldGroup = await target.GetAsync(fieldGroupId, cancellationToken);

            Assert.IsNotNull(actualFieldGroup);

            AssertFieldGroupsAreEqual(expectedFieldGroup, actualFieldGroup);
            foreach (var field in actualFieldGroup.Fields)
            {
                AssertFieldsAreEqual(expectedFields.First(expectedField => expectedField.Id == field.Id), field);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteByFieldGroupAsync_WhenFieldGroupIdLessOrEqualsZero_ShouldReturnArgumentException(int id)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(id, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task DeleteAsync_WhenFieldGroupWithIdExists_ShouldMarkAsDeleted(int fieldGroupId)
        {
            var expectedFieldGroup = CreateFieldGroup(fieldGroupId, 1);

            context.FieldGroups.Add(expectedFieldGroup);
            context.SaveChanges();

            await target.DeleteAsync(fieldGroupId, cancellationToken);

            var actualFieldGroup = context.FieldGroups.FirstOrDefault(fieldGroup => fieldGroup.Id == fieldGroupId);
            
            Assert.IsNotNull(actualFieldGroup);
            Assert.IsTrue(actualFieldGroup.IsDeleted);
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
                Type = FieldType.Decimal,
                Formula = "[1]+[2]"
            };
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

            // Ensure all properties are tested
		Assert.AreEqual(17, actual.GetType().GetProperties().Length);

            return true;
        }

        private DatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IFieldGroupRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
