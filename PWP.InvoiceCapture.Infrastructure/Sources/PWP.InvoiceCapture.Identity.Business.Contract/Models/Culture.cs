using System;
using System.Collections.Generic;
using System.Text;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class Culture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string NativeName { get; set; }
    }
}
