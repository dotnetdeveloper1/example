using System.IO;

namespace PWP.InvoiceCapture.Document.Business.Contract.Models
{
    public class CreateDocumentArgs
    {
        public string FileId { get; set; }
        public Stream FileContent { get;set; }
    }
}
