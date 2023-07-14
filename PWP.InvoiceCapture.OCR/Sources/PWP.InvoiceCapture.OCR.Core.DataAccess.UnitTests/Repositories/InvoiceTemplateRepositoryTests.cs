using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.DataAnalysis.DataAccess.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class InvoiceTemplateRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            cancellationToken = CancellationToken.None;
            context = (OCRDatabaseContext)contextFactory.Create();
            target = new InvoiceTemplateRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceTemplateRepository(null));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenTemplateIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.UpdateAsync(null, cancellationToken));
        }

        [DataRow(1)]
        [DataRow(3)]
        [DataRow(10)]
        [TestMethod]
        public async Task UpdateAsync_WhenTemplateIsNotNull_ShouldUpdate(int id)
        {
            var existingTemplate = CreateInvoiceTemplate(id);
            context.InvoiceTemplates.Add(existingTemplate);
            context.SaveChanges();

            var incrementedId = id + 1;
            var invoiceToUpdate = CreateInvoiceTemplate(incrementedId);
            invoiceToUpdate.Id = id;

            await target.UpdateAsync(invoiceToUpdate, cancellationToken);

            Assert.AreEqual(1, context.InvoiceTemplates.Count());

            var updatedInvoice = context.InvoiceTemplates
                .Include(contextItems => contextItems.GeometricFeatures)
                .FirstOrDefault();

            Assert.IsNotNull(updatedInvoice);

            AssertInvoiceTemplateAreEqual(invoiceToUpdate, updatedInvoice);
            AssertGeometricFeaturesCollectionIsEqual(existingTemplate.GeometricFeatures, updatedInvoice.GeometricFeatures);
        }

        [DataRow("")]
        [DataRow("  ")]
        [DataRow(null)]
        [TestMethod]
        public async Task GetAllAsync_WhenTenantIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string id)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.GetAllAsync(id, cancellationToken));
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllForTenantId()
        {
            var expectedTemplates = GenerateInvoiceTemplates(1, 3);

            context.InvoiceTemplates.AddRange(expectedTemplates);

            var templateForAnotherTenantId = CreateInvoiceTemplate(4);
            context.InvoiceTemplates.Add(templateForAnotherTenantId);
            context.SaveChanges();

            var actual = await target.GetAllAsync(tenantId, cancellationToken);

            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Count);

            AssertInvoiceTemplateCollectionsAreEqual(expectedTemplates, actual);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAll()
        {
            var expectedTemplates = GenerateInvoiceTemplates(1, 4);

            context.InvoiceTemplates.AddRange(expectedTemplates);
            context.SaveChanges();

            var actual = await target.GetAllAsync(cancellationToken);

            Assert.IsNotNull(actual);
            Assert.AreEqual(4, actual.Count);

            AssertInvoiceTemplateCollectionsAreEqual(expectedTemplates, actual);
        }

        [DataRow(0)]
        [DataRow(-1)]
        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsZeroOrNegative_ShouldThrowArgumentException(int id)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetByIdAsync(id, cancellationToken));
        }

        [DataRow(2)]
        [DataRow(3)]
        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnItemWithProvidedId(int id)
        {
            var expectedTemplates = GenerateInvoiceTemplates(1, 4);

            context.InvoiceTemplates.AddRange(expectedTemplates);
            context.SaveChanges();

            var actual = await target.GetByIdAsync(id, cancellationToken);

            Assert.IsNotNull(actual);
            AssertInvoiceTemplateAreEqual(expectedTemplates.FirstOrDefault(template => template.Id == id), actual);
        }

        [TestMethod]
        public async Task InsertAsync_WhenTemplateIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.InsertAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task InsertAsync_ShouldInsert()
        {
            var expectedTemplates = GenerateInvoiceTemplates(1, 3);
            context.InvoiceTemplates.AddRange(expectedTemplates);
            context.SaveChanges();

            var templateToInsert = GenerateInvoiceTemplates(4, 1).FirstOrDefault();

            await target.InsertAsync(templateToInsert, cancellationToken);

            Assert.AreEqual(4, context.InvoiceTemplates.Count());

            var actualInserted = context.InvoiceTemplates
               .Include(contextItems => contextItems.GeometricFeatures)
               .FirstOrDefault(contextItems => contextItems.Id == 4);
            
            var actualExisting = context.InvoiceTemplates
               .Include(contextItems => contextItems.GeometricFeatures)
               .OrderBy(contextItems => contextItems.Id)
               .Take(3)
               .ToList();

            Assert.IsNotNull(actualInserted);
            AssertInvoiceTemplateAreEqual(templateToInsert, actualInserted);
            AssertInvoiceTemplateCollectionsAreEqual(expectedTemplates, actualExisting);
        }


        private List<InvoiceTemplate> GenerateInvoiceTemplates(int startIndex, int count, bool forUniqTenant = true)
        {
            return Enumerable
                .Range(startIndex, count)
                .Select(index => forUniqTenant ? CreateInvoiceTemplate(index, tenantId) : CreateInvoiceTemplate(index))
                .ToList();
        }

        private void AssertInvoiceTemplateAreEqual(InvoiceTemplate expected, InvoiceTemplate actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.TenantId, actual.TenantId);
            Assert.AreEqual(expected.FormRecognizerModelId, actual.FormRecognizerModelId);
            Assert.AreEqual(expected.FormRecognizerId, actual.FormRecognizerId);
            Assert.AreEqual(expected.TrainingBlobUri, actual.TrainingBlobUri);
            Assert.AreEqual(expected.TrainingFileCount, actual.TrainingFileCount);
            AssertKeyWordCoordinatesAreEqual(expected.KeyWordCoordinates, actual.KeyWordCoordinates);
            Assert.AreEqual(8, expected.GetType().GetProperties().Length);
        }

        private void AssertInvoiceTemplateCollectionsAreEqual(List<InvoiceTemplate> expected, List<InvoiceTemplate> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());

            for (var i = 0; i < expected.Count(); i++)
            {
                AssertInvoiceTemplateAreEqual(expected[i], actual[i]);
            }
        }

        private void AssertKeyWordCoordinatesAreEqual(Dictionary<string, Coordinate> expected, Dictionary<string, Coordinate> actual)
        {
            var key = actual.Keys.FirstOrDefault();
            Assert.AreEqual(expected[key].Bottom, actual[key].Bottom);
            Assert.AreEqual(expected[key].Top, actual[key].Top);
            Assert.AreEqual(expected[key].Left, actual[key].Left);
            Assert.AreEqual(expected[key].Right, actual[key].Right);
        }

        private void AssertGeometricFeaturesCollectionIsEqual(GeometricFeatureCollection expectedCollection, GeometricFeatureCollection actualCollection)
        {
            Assert.AreEqual(expectedCollection.Id, actualCollection.Id);
            Assert.AreEqual(expectedCollection.AverageBlobHeight, actualCollection.AverageBlobHeight);
            Assert.AreEqual(expectedCollection.ConnectedComponentCount, actualCollection.ConnectedComponentCount);
            Assert.AreEqual(expectedCollection.ContourCount, actualCollection.ContourCount);
            Assert.AreEqual(expectedCollection.InvoiceTemplateId, actualCollection.InvoiceTemplateId);
            Assert.AreEqual(expectedCollection.InvoiceTemplate.Id, actualCollection.InvoiceTemplate.Id);
            Assert.AreEqual(expectedCollection.LineCount, actualCollection.LineCount);
            Assert.AreEqual(expectedCollection.PixelDensity, actualCollection.PixelDensity);

            Assert.AreEqual(8, expectedCollection.GetType().GetProperties().Length);
        }

        private InvoiceTemplate CreateInvoiceTemplate(int id, string tenantid = "")
        {
            return new InvoiceTemplate()
            {
                Id = id,
                TenantId = tenantid == string.Empty ? $"{tenantId}_{id}" : tenantid,
                TrainingFileCount = trainingFileCount + id,
                FormRecognizerModelId = $"{formRecognizerModelId}_{id}",
                FormRecognizerId = id,
                TrainingBlobUri = $"{trainingBlobUri}_{id}",
                KeyWordCoordinates = CreateKeyWordCoordinates(id),

                GeometricFeatures = CreateGeometricFeaturesCollection(id)
            };
        }

        private Dictionary<string, Coordinate> CreateKeyWordCoordinates(int id)
        {
            var result = new Dictionary<string, Coordinate>();
            result.Add($"{keyWordKey}_{id}", new Coordinate()
            {
                Bottom = coordinatesStartIndexBottom + id,
                Top = coordinatesStartIndexTop + id,
                Left = coordinatesStartIndexLeft + id,
                Right = coordinatesStartIndexRight + id,
            });

            return result;
        }

        private GeometricFeatureCollection CreateGeometricFeaturesCollection(int id)
        {
            return new GeometricFeatureCollection()
            {
                Id = geometricFeaturesStartIndex + id,
                PixelDensity = pixelDensity + id,
                LineCount = lineCount + id,
                ContourCount = contourCount + id,
                AverageBlobHeight = averageBlobHeight + id,
                ConnectedComponentCount = connectedComponentCount + id,
                InvoiceTemplateId = id
            };
        }

        private OCRDatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IInvoiceTemplateRepository target;
        private CancellationToken cancellationToken;

        private const string tenantId = "100";
        private const int trainingFileCount = 200;
        private const string formRecognizerModelId = "300";
        private const float coordinatesStartIndexBottom = 400;
        private const float coordinatesStartIndexTop = 500;
        private const float coordinatesStartIndexLeft = 600;
        private const float coordinatesStartIndexRight = 700;
        private const string trainingBlobUri = "someUri";
        private const string keyWordKey = "someKey";

        private const int geometricFeaturesStartIndex = 800;
        private const int pixelDensity = 1234;
        private const int lineCount = 2345;
        private const int contourCount = 3456;
        private const int averageBlobHeight = 4567;
        private const int connectedComponentCount = 5678;
    }
}
