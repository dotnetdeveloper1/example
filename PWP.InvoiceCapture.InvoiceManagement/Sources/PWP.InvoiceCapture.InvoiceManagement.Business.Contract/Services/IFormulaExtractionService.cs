using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IFormulaExtractionService
    {
        List<int> GetFieldIds(string formula);
        string GetNormalizedFormula(string formula, Dictionary<int, decimal> fieldValues);
        bool AreSquareBracketsBalanced(string formula);
    }
}
