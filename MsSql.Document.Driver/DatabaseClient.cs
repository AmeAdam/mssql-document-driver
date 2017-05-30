namespace MsSql.Document.Driver
{
    public class DatabaseClient
    {
        private readonly string host;

        public DatabaseClient(string host)
        {
            this.host = host;
        }

        public DocumentDatabase GetDatabase(string databaseName)
        {
            return new DocumentDatabase(host, databaseName);
        }
    }
}
