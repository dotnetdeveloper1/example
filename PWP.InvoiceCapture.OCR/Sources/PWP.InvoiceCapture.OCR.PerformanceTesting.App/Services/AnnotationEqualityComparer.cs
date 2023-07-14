using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services
{
    internal class AnnotationEqualityComparer : IEqualityComparer<Annotation>
    {
        public bool Equals(Annotation first, Annotation second) =>
            first.FieldType == second.FieldType &&
            first.FieldValue == second.FieldValue;

        public int GetHashCode(Annotation code) => code.GetHashCode();
    }
}
