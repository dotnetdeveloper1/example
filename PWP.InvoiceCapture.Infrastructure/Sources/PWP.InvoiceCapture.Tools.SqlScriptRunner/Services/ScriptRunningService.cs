using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.Tools.SqlScriptRunner.Contract;
using PWP.InvoiceCapture.Tools.SqlScriptRunner.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;

namespace PWP.InvoiceCapture.Tools.SqlScriptRunner.Services
{
    public class ScriptRunningService : IScriptRunningService
    {
        public ScriptRunningService(IOptions<DatabaseOptions> databaseOptionsAccessor)
        {
            connectionString = databaseOptionsAccessor.Value.ConnectionString;
        }

        public ExecutionResult RunScript(string script, List<string> databases)
        {
            foreach (var database in databases)
            {
                var result = RunQuery(script, null, database);
                if (!result.Succeed)
                {
                    return result;
                }
            }
            return new ExecutionResult() { Succeed = true };
        }

        public ExecutionResult RunScript(List<ScriptsPerDatabase> scriptsPerDatabase)
        {
            foreach (var database in scriptsPerDatabase)
            {
                foreach (var script in database.Scripts)
                {
                    var result = RunQuery(script.Script, script.FileName, database.DatabaseName);
                    if (!result.Succeed)
                    {
                        return result;
                    }
                }
            }
                
            return new ExecutionResult() { Succeed = true };
        }

        public List<string> GetTenantsDatabasesList(string script)
        {
            using (var connection = new SqlConnection())
            {
                SqlTransaction transaction = null;
                var result = new List<string>();
                try
                {
                    connection.ConnectionString = connectionString.Replace("{0}", "Tenants");

                    connection.Open();
                    transaction = connection.BeginTransaction();
                    using (var command = new SqlCommand(script, connection, transaction))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ordinalDatabaseName = reader.GetOrdinal("DatabaseName");
                            var databaseName = reader.GetString(ordinalDatabaseName);
                            result.Add(databaseName);
                        }
                        reader.Close();
                    }
                    transaction.Rollback();
                }
                catch (Exception exc)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    throw exc;
                }
                return result;
            }
        }

        public List<string> GetOtherDatabasesList(string script)
        {
            using (var connection = new SqlConnection())
            {
                SqlTransaction transaction = null;
                var result = new List<string>();
                try
                {
                    connection.ConnectionString = connectionString.Replace("{0}", "master");
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    using (var command = new SqlCommand(script, connection, transaction))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ordinalDatabaseName = reader.GetOrdinal("Name");
                            var databaseName = reader.GetString(ordinalDatabaseName);
                            result.Add(databaseName);
                        }
                        reader.Close();
                    }
                    transaction.Rollback();
                }
                catch (Exception exc)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    throw exc;
                }
                return result;
            }
        }

        private ExecutionResult RunQuery(string query, string scriptName, string databaseName)
        {
            using (var connection = new SqlConnection())
            {
                SqlTransaction transaction = null;
                try
                {
                    connection.ConnectionString = connectionString.Replace("{0}", databaseName);
                    connection.Open();
                    connection.ChangeDatabase(databaseName);
                    transaction = connection.BeginTransaction();
                    using (var command = new SqlCommand(query, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();

                    using (var command = new SqlCommand("INSERT INTO dbo.ExecutedScript (Name, Content) VALUES (@Name, @Content)", connection))
                    {
                        command.Parameters.Add("@Name", SqlDbType.VarChar).Value = scriptName ?? SqlString.Null;
                        command.Parameters.Add("@Content", SqlDbType.VarChar).Value = query;
                        command.ExecuteNonQuery();
                    }
                    return new ExecutionResult() { Succeed = true };
                }
                catch (Exception exception)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    return new ExecutionResult()
                    {
                        Succeed = false,
                        Exception = exception.ToString()
                    };
                }
            }
        }

        public List<string> GetExecutedScripts(string databaseName)
        {
            using (var connection = new SqlConnection())
            {
                var result = new List<string>();
                connection.ConnectionString = connectionString.Replace("{0}", databaseName);
                connection.Open();
                using (var command = new SqlCommand("SELECT NAME FROM dbo.ExecutedScript WHERE Name IS NOT NULL", connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nameOrdinal = reader.GetOrdinal("Name");
                        var name = reader.GetSqlString(nameOrdinal);
                        result.Add(name.ToString());
                    }
                    reader.Close();
                }
                return result;
            }
        }

        public List<ScriptsPerDatabase> GetScriptsToRunPerDatabase(List<string> databases, string scriptsFolderPath)
        {
            var result = new List<ScriptsPerDatabase>();
            foreach (var database in databases)
            {
                var executedScripts = GetExecutedScripts(database);
                var scriptsToRun = GetScriptsToRun(executedScripts, scriptsFolderPath);
                result.Add(new ScriptsPerDatabase()
                {
                    DatabaseName = database,
                    FolderName = scriptsFolderPath,
                    Scripts = scriptsToRun
                });
            }
            return result;
        }

        public string GetDryRunResult(List<ScriptsPerDatabase> scriptsPerDatabases)
        {
            var resultBuilder = new StringBuilder();
            resultBuilder.AppendLine($"Total: {scriptsPerDatabases.Sum(scriptPerDb => scriptPerDb.Scripts.Count())} scripts will be executed");
            foreach (var scriptToRun in scriptsPerDatabases)
            {
                resultBuilder.AppendLine();
                resultBuilder.AppendLine($"Database: {scriptToRun.DatabaseName}");
                foreach (var script in scriptToRun.Scripts)
                {
                    resultBuilder.AppendLine(script.FileName);
                }
            }
            resultBuilder.AppendLine();
            return resultBuilder.ToString().Substring(0, 500);
        }

        private List<ScriptToRun> GetScriptsToRun(List<string> executedScripts, string scriptsFolderPath)
        {
            var files = Directory.GetFiles(scriptsFolderPath);
            var result = new List<ScriptToRun>();
            foreach (var file in files)
            {
                var filepath = Path.GetFileName(file);
                if (executedScripts.Contains(filepath))
                {
                    continue;
                }
                result.Add(new ScriptToRun()
                {
                    FileName = Path.GetFileName(file),
                    Script = File.ReadAllText(file)
                });
            }
            return result;
        }

        private readonly string connectionString;
    }
}
