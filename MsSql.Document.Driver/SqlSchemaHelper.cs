using System.Data;
using System.Data.SqlClient;

namespace MsSql.Document.Driver
{
    internal static class SqlSchemaHelper
    {
        public static SqlCommand CreateDatabaseIfNotExist(string databaseName)
        {
            databaseName = databaseName.Replace('\'', '_');
            var cmd = new SqlCommand($"IF db_id(@databaseName) IS NULL BEGIN CREATE DATABASE {databaseName} END");
            cmd.Parameters.Add("@databaseName", SqlDbType.NVarChar).Value = databaseName;
            return cmd;
        }

        public static SqlCommand CheckIfDatabaseExist(string databaseName)
        {
            var cmd = new SqlCommand("select db_id(@databaseName)");
            cmd.Parameters.Add("@databaseName", SqlDbType.NVarChar).Value = databaseName;
            return cmd;
        }

        public static SqlCommand CheckIfTableExist(string collectionName)
        {
            var cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @collectionName");
            cmd.Parameters.Add("@collectionName", SqlDbType.NVarChar).Value = collectionName;
            return cmd;
        }

        public static SqlCommand DropDatabaseIfExist(string databaseName)
        {
            databaseName = databaseName.Replace('\'', '_');
            
            var cmd = new SqlCommand($"IF db_id(@databaseName) IS NOT NULL BEGIN ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {databaseName}; END");
            cmd.Parameters.Add("@databaseName", SqlDbType.NVarChar).Value = databaseName;
            return cmd;
        }

        public static SqlCommand CreateCollectionIfNotExist(string collectionName)
        {
            collectionName = collectionName.Replace('\'', '_');
            var cmd = new SqlCommand($"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @collectionName) BEGIN CREATE TABLE {collectionName} (id nvarchar(100) NOT NULL, json nvarchar(max) NOT NULL CONSTRAINT [PK_{collectionName}] PRIMARY KEY CLUSTERED (id ASC) ON [PRIMARY]) END");
            cmd.Parameters.Add("@collectionName", SqlDbType.NVarChar).Value = collectionName;
            return cmd;
        }

        public static SqlCommand DropCollectionIfExist(string collectionName)
        {
            collectionName = collectionName.Replace('\'', '_');
            var cmd = new SqlCommand($"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @collectionName) BEGIN DROP TABLE {collectionName} END");
            cmd.Parameters.Add("@collectionName", SqlDbType.NVarChar).Value = collectionName;
            return cmd;
        }
    }
}
