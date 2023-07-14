using System.Collections.Generic;

namespace PWP.InvoiceCapture.Tools.SqlScriptRunner.Models
{
    public class ScriptsPerDatabase
    {
        public string DatabaseName { get; set; }
        public string FolderName { get; set; }
        public List<ScriptToRun> Scripts { get; set; }
    }
}
