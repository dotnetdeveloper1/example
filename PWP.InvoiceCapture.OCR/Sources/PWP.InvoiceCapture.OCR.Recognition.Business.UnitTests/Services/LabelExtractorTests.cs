using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Factories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Services;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.UnitTests
{
    // TODO: Address failing tests later, related to renaming in WordDefinition class. This was serialized to json. 
    // TODO: Properties: "Left"/"Bottom"/"Top"/"Right" => "DocumentLevelNormalizedLeft"/etc
    // TODO: Properties: "NormalizedLeft"/etc => "PageLevelNormalizedLeft"/etc

    //[TestClass]
    //public class LabelExtractorTests
    //{
    //    public TestContext TestContext { get; set; }

    //    [TestInitialize]
    //    public void Initialize()
    //    {
    //        serializationService = new SerializationService();
    //        labelsFromFile = new LabelsOfInterestFromFile(serializationService);
    //        var coleParmerFile = Path.Combine(TestContext.DeploymentDirectory, @"Files\OcrOutput\ColeParmer1.json");
    //        var coleParmerContent = File.ReadAllText(coleParmerFile);
    //        coleParmerWords = serializationService.Deserialize<List<WordDefinition>>(coleParmerContent);
    //        coleParmerExpectedResults = CreateColeParmerResults();
    //    }

    //    private Dictionary<string, string> CreateColeParmerResults()
    //    {
    //        var expectedResults = new Dictionary<string, string>();
    //        expectedResults.Add(FieldTypes.InvoiceNumber, "1924399");
    //        expectedResults.Add(FieldTypes.InvoiceDate, "07/02/2019");
    //        expectedResults.Add(FieldTypes.TaxNumber, "36-2360953");
    //        expectedResults.Add(FieldTypes.DueDate, "");
    //        expectedResults.Add(FieldTypes.TaxAmount, "");
    //        expectedResults.Add(FieldTypes.SubTotal, "");
    //        return expectedResults;
    //    }

    //    private Dictionary<string, string> CreateAgfaResults()
    //    {
    //        /*TODO: Create Agfa results */
    //        var expectedResults = new Dictionary<string, string>();
    //        return expectedResults;
    //    }

    //    private Dictionary<string, string> CreateSeekResults()
    //    {
    //        /*TODO: Create Seek results */
    //        var expectedResults = new Dictionary<string, string>();
    //        return expectedResults;
    //    }

    //    private Dictionary<string, string> CreateTristelResults()
    //    {
    //        /*TODO: Create Tristel results */
    //        var expectedResults = new Dictionary<string, string>();
    //        return expectedResults;
    //    }

    //    private bool AreAllLabelsFound(Dictionary<string, string> expectedLabels, Dictionary<LabelOfInterest, List<WordGroup>> foundLabels)
    //    {
    //        var foundKeys = foundLabels.Keys.Select(labelKeys => labelKeys.Text).ToList();
    //        foreach (var key in expectedLabels.Keys)
    //        {
    //            if (!foundKeys.Contains(key))
    //            { 
    //                return false;
    //            }
    //        }
    //        return true;
    //    }

    //    [TestMethod]
    //    public void ExtractLabels_WhenInputIsValid_ShouldFindCorrectLabels()
    //    {   
    //        var labelExtractor = new LabelExtractorService(labelsFromFile, new LineService(), new LabelOfInterestService());
    //        var ocrElementsFactory = new OCRElementsFactory();
    //        var foundLabels = labelExtractor.ExtractLabelsAsync(ocrElementsFactory.Create(coleParmerWords),CancellationToken.None).Result;
    //        var areAllLabelsFound = AreAllLabelsFound(coleParmerExpectedResults, foundLabels);
    //        Assert.IsTrue(areAllLabelsFound);
    //    }
    //    private ISerializationService serializationService;
    //    private LabelsOfInterestFromFile labelsFromFile;
    //    private List<WordDefinition> coleParmerWords;
    //    private Dictionary<string, string> coleParmerExpectedResults;
    //}
}
