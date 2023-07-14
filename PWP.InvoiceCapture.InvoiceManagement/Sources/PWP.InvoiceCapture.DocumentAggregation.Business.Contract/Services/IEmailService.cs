using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Services
{
    public interface IEmailService
    {
        string FindEmail(string text);
    }
}
