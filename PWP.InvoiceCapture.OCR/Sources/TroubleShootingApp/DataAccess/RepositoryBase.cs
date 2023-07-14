using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace TroubleShootingApp.DataAccess
{
    public class RepositoryBase<T>
    {
        protected List<T> GetAll(string sqlStatement, string connectionString, Func<dynamic, T> mappingFunction)
        {
            SqlConnection connection;
            connection = new SqlConnection(connectionString);
            string sql = sqlStatement;
            List<T> ret = new List<T>();
            connection.Open();
            var command = new SqlCommand(sql, connection);
            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                dynamic newobj = new ExpandoObject();
                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    AddProperty(newobj, dataReader.GetName(i), dataReader.GetValue(i));
                }
                ret.Add(mappingFunction(newobj));
            }
            dataReader.Close();
            command.Dispose();
            connection.Close();
            return ret;
        }

        protected async Task<List<T>> GetAllAsync(string sqlStatement, string connectionString, Func<dynamic, T> mappingFunction)
        {
            SqlConnection connection;
            connection = new SqlConnection(connectionString);
            string sql = sqlStatement;
            List<T> ret = new List<T>();
            connection.Open();
            var command = new SqlCommand(sql, connection);
            var dataReader = await command.ExecuteReaderAsync();
            while (await dataReader.ReadAsync())
            {
                dynamic newobj = new ExpandoObject();
                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    AddProperty(newobj, dataReader.GetName(i), dataReader.GetValue(i));
                }
                ret.Add(mappingFunction(newobj));
            }
            dataReader.Close();
            command.Dispose();
            connection.Close();
            return ret;
        }

        public void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var exDict = expando as IDictionary<string, object>;
            if (exDict.ContainsKey(propertyName))
            {
                exDict[propertyName] = propertyValue;
            }
            else
            {
                exDict.Add(propertyName, propertyValue);
            }
        }
    }
}
