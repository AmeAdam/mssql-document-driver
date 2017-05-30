using System.Collections.Generic;
using System.Data.SqlClient;

namespace MsSql.Document.Driver
{
    public class DocumentDatabase
    {
        private readonly string connectionString;

        internal DocumentDatabase(string host, string databaseName)
        {
            connectionString = $"Server = {host}; Database = {databaseName}; Trusted_Connection = True;";
        }

        public DocumentCollection<T> GetCollection<T>(string name) where T : IIdentifiable
        {
            return new DocumentCollection<T>(this, name);
        }

        internal void ExecuteCommand(SqlCommand cmd)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();
            }
        }

        internal IEnumerable<(string id, string json)> ExecuteReader(SqlCommand cmd)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                cmd.Connection = connection;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return (reader.GetString(0), reader.GetString(1));
                    }
                }
            }
        }
    }
}