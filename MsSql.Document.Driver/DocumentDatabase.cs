using System.Collections.Generic;
using System.Data.SqlClient;

namespace MsSql.Document.Driver
{
    public class DocumentDatabase
    {
        private readonly string host;
        private readonly string databaseName;
        private readonly string connectionString;

        internal DocumentDatabase(string host, string databaseName)
        {
            this.host = host;
            this.databaseName = databaseName;
            connectionString = $"Server = {host}; Database = {databaseName}; Trusted_Connection = True;";
            ExecuteCommandOnMaster(SqlSchemaHelper.CreateDatabaseIfNotExist(databaseName));
        }

        public void Drop()
        {
            ExecuteCommandOnMaster(SqlSchemaHelper.DropDatabaseIfExist(databaseName));
        }

        public DocumentCollection<T> GetCollection<T>(string name) where T : IIdentifiable
        {
            return new DocumentCollection<T>(this, name);
        }

        internal void ExecuteCommandOnMaster(SqlCommand cmd)
        {
            using (var masterConnection = new SqlConnection($"Server = {host}; Database = master; Trusted_Connection = True;"))
            {
                masterConnection.Open();
                using (cmd)
                {
                    cmd.Connection = masterConnection;
                    cmd.ExecuteNonQuery();
                }
            }
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