using System.IO;

namespace PWP.InvoiceCapture.Document.Business.Contract.Models
{
    public class GetDocumentStreamResult
    {
        public Stream FileStream { get; set; }
        public long Length { get; set; }
        public string ContentType { get; set; }
    }
}
