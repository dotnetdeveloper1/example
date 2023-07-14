using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Database;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class LabelOfInterestRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryRecognitionDatabaseContextFactory();
            cancellationToken = CancellationToken.None;
            context = (RecognitionDatabaseContext)contextFactory.Create();
            target = new LabelOfInterestRepository(contextFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Dispose();
        }

        [TestMethod]
        public void Instance_WhenRecognitionDatabaseContextFactoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LabelOfInterestRepository(null));
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAll()
        {
            var labels = Enumerable
                             .Range(1, 3)
                             .Select(index => GenerateLabelOfInterest(index))
                             .ToList();
            context.LabelsOfInterest.AddRange(labels);
            await context.SaveChangesAsync(cancellationToken);

            var actual = await target.GetAllAsync(cancellationToken);

            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Count());
            AssertLabelOfInterestCollectionsAreEqual(labels, actual.ToList());
        }

        private LabelOfInterest GenerateLabelOfInterest(int id)
        {
            return new LabelOfInterest()
            {
                Id = id,
                ExpectedType = expectedType,
                Text = $"{text}_{id}",
                Synonyms = new List<LabelSynonym>
                {
                    new LabelSynonym()
                    {
                        LabelOfInterestId = id,
                        Text = $"{synonymText}_{id}",
                        Id = id + synonumIncrement,
                        UseInTemplateComparison = true
                    }
                },
                MockedErrors = new List<string>
                {
                    mockedErrorText
                },
                FallBackLabels = new List<LabelOfInterest>
                {
                   new LabelOfInterest()
                   {
                       Id = id + fallbackIncrement
                   }
                },
                Keywords = new List<LabelKeyWord>
                {
                    new LabelKeyWord
                    {
                        Id = id + keyWordsIncrement,
                        LabelOfInterestId = id,
                        Text = $"{keywordText}_{id}"
                    }
                },
                UseAbsoluteComparison = true

            };
        }

        private void AssertLabelOfInterestCollectionsAreEqual(List<LabelOfInterest> expected, List<LabelOfInterest> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());

            for (var i = 0; i < expected.Count(); i++)
            {
                AssertLabelOfInterestAreEqual(expected[i], actual[i]);
            }
        }

        private void AssertLabelOfInterestAreEqual(LabelOfInterest expected, LabelOfInterest actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Text, actual.Text);
            Assert.AreEqual(expected.Synonyms.Count(), actual.Synonyms.Count());
            AssertSynonymAreEqual(expected.Synonyms.FirstOrDefault(), actual.Synonyms.FirstOrDefault());
            Assert.AreEqual(expected.Keywords.Count(), actual.Keywords.Count());
            Assert.AreEqual(expected.Keywords.FirstOrDefault().Id, actual.Keywords.FirstOrDefault().Id);
            Assert.AreEqual(expected.Keywords.FirstOrDefault().LabelOfInterestId, actual.Keywords.FirstOrDefault().LabelOfInterestId);
            Assert.AreEqual(expected.Keywords.FirstOrDefault().Text, actual.Keywords.FirstOrDefault().Text);
            Assert.AreEqual(expected.UseAbsoluteComparison, actual.UseAbsoluteComparison);

            Assert.AreEqual(8, expected.GetType().GetProperties().Length);

        }

        private void AssertSynonymAreEqual(LabelSynonym expected, LabelSynonym actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.LabelOfInterestId, actual.LabelOfInterestId);
            Assert.AreEqual(expected.Text, actual.Text);
            Assert.AreEqual(expected.UseInTemplateComparison, actual.UseInTemplateComparison);
        }

        private IRecognitionDatabaseContext context;
        private IRecognitionDatabaseContextFactory contextFactory;
        private ILabelOfInterestRepository target;
        private CancellationToken cancellationToken;

        private const int synonumIncrement = 100;
        private const int fallbackIncrement = 1000;
        private const int keyWordsIncrement = 5000;
        private const DataType expectedType = DataType.String;
        private const string text = "someStringText";
        private const string synonymText = "someStringText synonym";
        private const string mockedErrorText = "some mocked error text";
        private const string keywordText = "some mocked keyword text";
    }
}
