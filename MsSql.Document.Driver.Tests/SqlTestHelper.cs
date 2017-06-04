using System;
using System.Data.SqlClient;

namespace MsSql.Document.Driver.Tests
{
    internal class SqlTestHelper
    {
        private readonly string masterConnectionString = $"Server = {Settings.Host}; Database = master; Trusted_Connection = True;";
        private readonly string connectionString;
        private readonly string databaseName;

        public SqlTestHelper(string databaseName)
        {
            this.databaseName = databaseName;
            connectionString = $"Server = {Settings.Host}; Database = {databaseName}; Trusted_Connection = True;";
        }

        public void DropDatabase()
        {
            Execute(masterConnectionString, SqlSchemaHelper.DropDatabaseIfExist(databaseName));
        }

        public bool SqlDatabaseExist(string database)
        {
            return ExecuteScalar<short>(masterConnectionString, SqlSchemaHelper.CheckIfDatabaseExist(database)) != null;
        }

        public bool SqlTableExist(string collectionName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = SqlSchemaHelper.CheckIfTableExist(collectionName))
                {
                    cmd.Connection = connection;
                    var res = cmd.ExecuteScalar();
                    return res != DBNull.Value && res != null;
                }
            }
        }

        public static T? ExecuteScalar<T>(string connectionString, SqlCommand cmd) where T : struct
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (cmd)
                {
                    cmd.Connection = connection;
                    var res = cmd.ExecuteScalar() as T?;
                    return res;
                }
            }
        }

        public static void Execute(string connectionString, SqlCommand cmd)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (cmd)
                {
                    cmd.Connection = connection;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
