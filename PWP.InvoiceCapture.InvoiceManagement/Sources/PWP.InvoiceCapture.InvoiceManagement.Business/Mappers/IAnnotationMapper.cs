using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Mappers
{
    internal interface IAnnotationMapper
    {
        InvoiceField ToInvoiceField(int invoiceId, Annotation annotation, List<Field> fields, CultureInfo cultureInfo);
        InvoiceLine ToInvoiceLine(LineAnnotation lineAnnotation, int invoiceId, CultureInfo cultureInfo);
    }
}
