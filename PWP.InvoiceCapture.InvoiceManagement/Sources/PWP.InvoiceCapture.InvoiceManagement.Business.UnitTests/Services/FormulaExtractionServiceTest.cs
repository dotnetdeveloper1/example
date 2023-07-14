using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FormulaExtractionServiceTest
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new FormulaExtractionService();
        }

        [TestMethod]
        [DataRow("[1]+[2]-[3]*[4]")]
        [DataRow("([1]+[2]-[3])*[4]")]
        [DataRow("([1]+[2])-([3]-[4])")]
        public void AreSquareBracketBalanced_IsBalanced_ShouldReturnTrue(string formula)
        {
            var result = target.AreSquareBracketsBalanced(formula);

            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("[[1]+[2]-[3]*[4]]")]
        [DataRow("[[1]+[2]-[3]*[4]")]
        [DataRow("[1]+2]-[3]*[4]")]
        [DataRow("[1]+[[2]-[3]]*[4]")]
        [DataRow("[1]+[[2-3]]*[4]")]
        public void AreSquareBracketBalanced_IsNotBalanced_ShouldReturnFalse(string formula)
        {
            var result = target.AreSquareBracketsBalanced(formula);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetNormalizedFormula_ShouldReturnNormalizedFormula()
        {
            var formula = "[12]+[13]-[14]*[15]";
            var fieldValues = new Dictionary<int, decimal> { { 12, 1 }, { 13, 2 }, { 14, 3 }, { 15, 4 } };

            var result = target.GetNormalizedFormula(formula, fieldValues);

            Assert.IsNotNull(result);
            Assert.AreEqual("1+2-3*4", result);
        }

        [TestMethod]
        [DataRow("[2]*[1]")]
        [DataRow("6+[2]*[1]")]
        [DataRow("[2]*[1]+[1]-3")]
        public void GetFieldIds_ShouldReturnFieldIds(string formula)
        {
            var expectedList = new List<int> { 1, 2 };

            var result = target.GetFieldIds(formula);

            result.Sort();

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expectedList, result);
        }

        private IFormulaExtractionService target;
    }
}
