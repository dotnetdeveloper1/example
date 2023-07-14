using System;
using System.Collections.Generic;
using System.Text;

namespace TroubleShootingApp.Models
{
    public class Invoice
    {
        public string Name { get; set; }
        public int StatusId { get; set; }
        public string FileName { get; set; }
        public string FileId { get; set; }
        public string TemplateId { get; set; }
    }
}
