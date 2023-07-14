using PWP.InvoiceCapture.Tools.SqlScriptRunner.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.Tools.SqlScriptRunner.Contract
{
    public interface IScriptRunningService
    {
        ExecutionResult RunScript(string script, List<string> databases);
        ExecutionResult RunScript(List<ScriptsPerDatabase> scripts);
        List<string> GetTenantsDatabasesList(string script);
        List<string> GetOtherDatabasesList(string script);
        string GetDryRunResult(List<ScriptsPerDatabase> scripts);
        List<ScriptsPerDatabase> GetScriptsToRunPerDatabase(List<string> databases, string scriptsFolderPath);
    }
}
