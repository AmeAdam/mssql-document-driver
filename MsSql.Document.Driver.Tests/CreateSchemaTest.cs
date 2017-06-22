using MsSql.Document.Driver.Tests.TestModel;
using NUnit.Framework;

namespace MsSql.Document.Driver.Tests
{
    [TestFixture]
    public class CreateSchemaTest
    {
        private DatabaseClient dbClient;
        private string databaseName = "TestDatabase";
        private SqlTestHelper sqlHelper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            dbClient = new DatabaseClient(Settings.Host);
            sqlHelper = new SqlTestHelper(databaseName);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            sqlHelper.DropDatabase();
        }

        [Test, Order(1)]
        public void CreateDatabase()
        {
            Assert.IsFalse(sqlHelper.SqlDatabaseExist(databaseName));
            var database = dbClient.GetDatabase(databaseName);
            Assert.IsTrue(sqlHelper.SqlDatabaseExist(databaseName));

            Assert.IsNotNull(dbClient.GetDatabase(databaseName));
            database.Drop();
            Assert.IsFalse(sqlHelper.SqlDatabaseExist(databaseName));
        }

        [Test, Order(2)]
        public void CreateCollection()
        {
            var database = dbClient.GetDatabase(databaseName);
            Assert.IsFalse(sqlHelper.SqlTableExist("Person"));
            var collection = database.GetCollection<Person>("Person");

            Assert.IsTrue(sqlHelper.SqlTableExist("Person"));
            Assert.IsNotNull(database.GetCollection<Person>("Person"));

            collection.Drop();
            Assert.IsFalse(sqlHelper.SqlTableExist("Person"));
        }


    }
}
