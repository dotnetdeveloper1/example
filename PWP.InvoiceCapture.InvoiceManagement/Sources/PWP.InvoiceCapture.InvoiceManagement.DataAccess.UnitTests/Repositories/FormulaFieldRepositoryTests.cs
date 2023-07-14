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
    public class FormulaFieldRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            context = (DatabaseContext)contextFactory.Create();
            target = new FormulaFieldRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new FormulaFieldRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenOperandIdsAreNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task CreateAsync_WhenFieldIdIsLessOrEqualsZero_ShouldThrowArgumentNullException(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.CreateAsync(fieldId, new List<int>(), cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task CreateAsync_WhenOperandIdsAreIsLessOrEqualsZero_ShouldThrowArgumentNullException(int operandId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.CreateAsync(1, new List<int>() { operandId }, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task CreateAsync_WhenOperandIdAreIsLessOrEqualsZero_ShouldThrowArgumentNullException(int operandId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.CreateAsync(1, operandId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task CreateAsync_WhenOperandIdIsLessOrEqualsZero_ShouldThrowArgumentNullException(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.CreateAsync(fieldId, 1, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 1, 1, 1)]
        [DataRow(10, 2, 4, 2)]
        [DataRow(100, 3, 5, 5)]
        public async Task CreateAsync_WhenOperandFieldIdIsCorrect_ShouldSaveFormulaField(int fieldId, int orderNumber, int groupId, int operandFieldId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingField = CreateField(fieldId, groupId);
            var existingOperandField = CreateField(operandFieldId, groupId);

            context.FieldGroups.Add(existingFieldGroup);
            context.Fields.Add(existingField);

            context.SaveChanges();

            await target.CreateAsync(existingField.Id, existingOperandField.Id, cancellationToken);

            var actualFormulaField = context.FormulaFields.FirstOrDefault();

            Assert.IsNotNull(actualFormulaField);
            Assert.AreNotEqual(0, actualFormulaField.Id);
            Assert.AreNotEqual(0, actualFormulaField.ResultFieldId == fieldId);
            Assert.AreNotEqual(default, actualFormulaField.CreatedDate);
            Assert.AreNotEqual(default, actualFormulaField.ModifiedDate);
        }

        [TestMethod]
        [DataRow(1, 1, 1, 1)]
        [DataRow(10, 2, 4, 2)]
        [DataRow(100, 3, 5, 5)]
        public async Task CreateAsync_WhenOperandFieldIdsAreCorrect_ShouldSaveFormulaField(int fieldId, int orderNumber, int groupId, int operandFieldId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingField = CreateField(fieldId, groupId);
            var existingOperandField1 = CreateField(operandFieldId, groupId);
            var existingOperandField2 = CreateField(operandFieldId + 1, groupId);

            context.FieldGroups.Add(existingFieldGroup);
            context.Fields.Add(existingField);

            context.SaveChanges();

            await target.CreateAsync(existingField.Id, new List<int>() { existingOperandField1.Id, existingOperandField2.Id }, cancellationToken);

            var actualFormulaFields = context.FormulaFields.Where(formulaField => formulaField.ResultFieldId == fieldId).ToList();

            foreach (var actualFormulaField in actualFormulaFields)
            {
                Assert.IsNotNull(actualFormulaField);
                Assert.AreNotEqual(0, actualFormulaField.Id);
                Assert.AreNotEqual(0, actualFormulaField.ResultFieldId == fieldId);
                Assert.AreNotEqual(default, actualFormulaField.CreatedDate);
                Assert.AreNotEqual(default, actualFormulaField.ModifiedDate);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetByOperandFieldIdAsync_WhenOperandFieldIdIsLessOrEqualsZero_ShouldThrowException(int operandId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetByOperandFieldIdAsync(operandId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 2, 3, 4)]
        [DataRow(10, 2, 4, 2)]
        [DataRow(100, 3, 5, 6)]
        public async Task GetByOperandFieldIdAsync_WhenOperandFieldIdsAreCorrect_ShouldSaveFormulaField(int resultFieldId, int orderNumber, int groupId, int operandFieldId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingField = CreateField(resultFieldId, groupId);
            var existingOperandField1 = CreateField(operandFieldId, groupId);
            var existingOperandField2 = CreateField(operandFieldId + 1, groupId);
            var formulaField1 = CreateFormulaField(1, existingOperandField1.Id, resultFieldId);
            var formulaField2 = CreateFormulaField(2, existingOperandField2.Id, resultFieldId);
            var formulaField3 = CreateFormulaField(3, existingOperandField2.Id, existingOperandField1.Id);

            context.FieldGroups.Add(existingFieldGroup);
            context.Fields.Add(existingField);
            context.Fields.Add(existingField);
            context.FormulaFields.Add(formulaField1);
            context.FormulaFields.Add(formulaField2);
            context.FormulaFields.Add(formulaField3);

            context.SaveChanges();

            var operandFormulaFields = await target.GetByOperandFieldIdAsync(existingOperandField2.Id, cancellationToken);

            foreach (var actualFormulaField in operandFormulaFields)
            {
                var expectedFormulaField = context.FormulaFields.First(formulaField => formulaField.Id == actualFormulaField.Id);
                AssertFormulaFieldsAreEqual(expectedFormulaField, actualFormulaField);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetByResultFieldIdAsync_WhenResultOperandFieldIdIsLessOrEqualsZero_ShouldThrowException(int operandId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetByResultFieldIdAsync(operandId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 2, 3, 4)]
        [DataRow(10, 2, 4, 2)]
        [DataRow(100, 3, 5, 6)]
        public async Task GetByResultFieldIdAsync_WhenOperandFieldIdsAreCorrect_ShouldSaveFormulaField(int resultFieldId, int orderNumber, int groupId, int operandFieldId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingField = CreateField(resultFieldId, groupId);
            var existingOperandField1 = CreateField(operandFieldId, groupId);
            var existingOperandField2 = CreateField(operandFieldId + 1, groupId);
            var formulaField1 = CreateFormulaField(1, existingOperandField1.Id, resultFieldId);
            var formulaField2 = CreateFormulaField(2, existingOperandField2.Id, resultFieldId);
            var formulaField3 = CreateFormulaField(3, existingOperandField2.Id, existingOperandField1.Id);

            context.FieldGroups.Add(existingFieldGroup);
            context.Fields.Add(existingField);
            context.Fields.Add(existingField);
            context.FormulaFields.Add(formulaField1);
            context.FormulaFields.Add(formulaField2);
            context.FormulaFields.Add(formulaField3);
            context.SaveChanges();

            var operandFormulaFields = await target.GetByResultFieldIdAsync(resultFieldId, cancellationToken);

            foreach (var actualFormulaField in operandFormulaFields)
            {
                var expectedFormulaField = context.FormulaFields.First(formulaField => formulaField.Id == actualFormulaField.Id);
                AssertFormulaFieldsAreEqual(expectedFormulaField, actualFormulaField);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteAllByResultFieldIdAsync_WhenFieldIdIsLessOrEqualsZero_ShouldThrowException(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.DeleteAllByResultFieldIdAsync(fieldId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 2, 3, 4)]
        [DataRow(10, 2, 4, 2)]
        [DataRow(100, 3, 5, 6)]
        public async Task DeleteAllByResultFieldIdAsync_WhenOperandFieldIdsAreCorrect_ShouldDeleteFormulaFields(int resultFieldId, int orderNumber, int groupId, int operandFieldId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingField = CreateField(resultFieldId, groupId);
            var existingOperandField1 = CreateField(operandFieldId, groupId);
            var existingOperandField2 = CreateField(operandFieldId + 1, groupId);
            var formulaField1 = CreateFormulaField(1, existingOperandField1.Id, resultFieldId);
            var formulaField2 = CreateFormulaField(2, existingOperandField2.Id, resultFieldId);
            var formulaField3 = CreateFormulaField(3, existingOperandField2.Id, existingOperandField1.Id);

            context.FieldGroups.Add(existingFieldGroup);
            context.Fields.Add(existingField);
            context.FormulaFields.Add(formulaField1);
            context.FormulaFields.Add(formulaField2);
            context.FormulaFields.Add(formulaField3);
            context.SaveChanges();

            await target.DeleteAllByResultFieldIdAsync(resultFieldId, cancellationToken);

            Assert.AreEqual(context.FormulaFields.Count(formulaField => formulaField.ResultFieldId == resultFieldId), 0);
            Assert.AreEqual(context.FormulaFields.Count(formulaField => formulaField.ResultFieldId == existingOperandField1.Id), 1);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UsedAsOperandInFormulaAsync_WhenResultFieldIdIsLessOrEqualsZero_ShouldArgumentExceptionException(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UsedAsOperandInFormulaAsync(fieldId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 2, 3, 4)]
        [DataRow(10, 2, 4, 2)]
        [DataRow(100, 3, 5, 6)]
        public async Task UsedAsOperandInFormulaAsync_WhenFormulaFieldHasReferenceToField_ShouldReturnTrue(int resultFieldId, int orderNumber, int groupId, int operandFieldId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingField = CreateField(resultFieldId, groupId);
            var existingOperandField1 = CreateField(operandFieldId, groupId);
            var formulaField1 = CreateFormulaField(1, existingOperandField1.Id, resultFieldId);

            context.FieldGroups.Add(existingFieldGroup);
            context.Fields.Add(existingField);
            context.Fields.Add(existingField);
            context.FormulaFields.Add(formulaField1);

            context.SaveChanges();

            var wasUsed = await target.UsedAsOperandInFormulaAsync(existingOperandField1.Id, cancellationToken);
            var wasUsedNotExistingOperand = await target.UsedAsOperandInFormulaAsync(existingOperandField1.Id + 1000, cancellationToken);

            Assert.IsTrue(wasUsed);
            Assert.IsFalse(wasUsedNotExistingOperand);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UsedAsResultFieldInFormulaAsync_WhenResultFieldIdIsLessOrEqualsZero_ShouldArgumentExceptionException(int fieldId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UsedAsResultFieldInFormulaAsync(fieldId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1, 2, 3, 4)]
        [DataRow(10, 2, 4, 2)]
        [DataRow(100, 3, 5, 6)]
        public async Task UsedAsResultFieldInFormulaAsync_WhenFormulaFieldHasReferenceToField_ShouldReturnTrue(int resultFieldId, int orderNumber, int groupId, int operandFieldId)
        {
            var existingFieldGroup = CreateFieldGroup(groupId, orderNumber);
            var existingField = CreateField(resultFieldId, groupId);
            var existingOperandField1 = CreateField(operandFieldId, groupId);
            var formulaField1 = CreateFormulaField(1, existingOperandField1.Id, resultFieldId);

            context.FieldGroups.Add(existingFieldGroup);
            context.Fields.Add(existingField);
            context.Fields.Add(existingField);
            context.FormulaFields.Add(formulaField1);

            context.SaveChanges();

            var wasUsed = await target.UsedAsOperandInFormulaAsync(existingOperandField1.Id, cancellationToken);
            var wasUsedNotExistingOperand = await target.UsedAsOperandInFormulaAsync(resultFieldId + 1000, cancellationToken);

            Assert.IsTrue(wasUsed);
            Assert.IsFalse(wasUsedNotExistingOperand);
        }

        private void AssertFormulaFieldsAreEqual(FormulaField expected, FormulaField actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.OperandFieldId, actual.OperandFieldId);
            Assert.AreEqual(expected.ResultFieldId, actual.ResultFieldId);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);

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

        private FormulaField CreateFormulaField(int id, int operandFieldId, int resultFieldId)
        {
            return new FormulaField
            {
                Id = id,
                OperandFieldId = operandFieldId,
                ResultFieldId = resultFieldId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }

        private DatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IFormulaFieldRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
