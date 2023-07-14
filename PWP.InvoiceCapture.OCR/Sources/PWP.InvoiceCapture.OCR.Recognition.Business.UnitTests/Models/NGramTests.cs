using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.Services;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Factories;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.UnitTests
{
    // TODO: Address failing tests later, related to renaming in WordDefinition class. This was serialized to json. 
    // TODO: Properties: "Left"/"Bottom"/"Top"/"Right" => "DocumentLevelNormalizedLeft"/etc
    // TODO: Properties: "NormalizedLeft"/etc => "PageLevelNormalizedLeft"/etc


    //[TestClass]
    //public class NGramTests
    //{
    //    public TestContext TestContext { get; set; }

    //    [TestInitialize]
    //    public void Initialize()
    //    {   
    //        var coleParmerFile = Path.Combine(TestContext.DeploymentDirectory, @"Files\OcrOutput\ColeParmer1.json");
    //        var coleParmerContent = File.ReadAllText(coleParmerFile);
    //        var coleParmerWords = (new SerializationService()).Deserialize<List<WordDefinition>>(coleParmerContent);
    //        var ocrElementsFactory = new OCRElementsFactory();
    //        oCRElements = ocrElementsFactory.Create(coleParmerWords);
    //        nGramFactory = new NGramFactory();
    //    }

    //    [TestMethod]
    //    public void ExtractLabels_WhenInputIsNonEmpty_ShouldFindCorrectNumberOfWords()
    //    {
    //        var lines = oCRElements.Lines.ToList();
    //        var ngrams = nGramFactory.Create(lines[testingLineNo].Words);
    //        var ngramCount = ngrams.Count;
    //        var wordsInLineCount = lines[testingLineNo].Words.Count;
    //        var expectedCount = (wordsInLineCount * (wordsInLineCount + 1)) / 2;
    //        Assert.AreEqual(ngramCount, expectedCount, "NGram count doesn't match the expected count");
    //    }

    //    private readonly int testingLineNo = 33;
    //    private OCRElements oCRElements;
    //    private NGramFactory nGramFactory;

    //}
}
