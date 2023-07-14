using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class FormulaExtractionService : IFormulaExtractionService
    {
        public List<int> GetFieldIds(string formula)
        {
            var formulaFieldsIds = new List<int>();
            Match match = formulaRegex.Match(formula);

            while (match.Success)
            {
                int fieldId;

                if (int.TryParse(match.Groups[1].Value, out fieldId))
                {
                    formulaFieldsIds.Add(fieldId);
                }

                match = match.NextMatch();
            }

            return formulaFieldsIds.Distinct().ToList();
        }

        public string GetNormalizedFormula(string formula, Dictionary<int, decimal> fieldValues)
        {
            Match match = formulaRegex.Match(formula);

            while (match.Success)
            {
                int fieldId;

                if (int.TryParse(match.Groups[1].Value, out fieldId))
                {
                    formula = formula.Replace(match.Value, fieldValues[fieldId].ToString());
                }

                match = match.NextMatch();
            }

            return formula;
        }

        public bool AreSquareBracketsBalanced(string formula)
        {
            var formulaWithoutIds = formulaRegex.Replace(formula, "");
            return !formulaWithoutIds.Contains("[") && !formulaWithoutIds.Contains("]");
        }

        private readonly Regex formulaRegex = new Regex(@"\[(\d+?)\]", RegexOptions.IgnoreCase);
    }
}
