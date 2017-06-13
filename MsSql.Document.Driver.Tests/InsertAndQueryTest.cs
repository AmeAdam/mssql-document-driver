using System;
using System.Linq;
using MsSql.Document.Driver.Tests.TestModel;
using NUnit.Framework;

namespace MsSql.Document.Driver.Tests
{
    [TestFixture]
    public class InsertAndQueryTest
    {
        private DocumentDatabase db;
        private DocumentCollection<TestUser> users;
        private SqlTestHelper sqlHelper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            sqlHelper = new SqlTestHelper("InsertSomeDataTest");
            db = new DatabaseClient(Settings.Host).GetDatabase("InsertSomeDataTest");
            users = db.GetCollection<TestUser>("users");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            sqlHelper.DropDatabase();
        }

        [Test]
        public void InsertSomeData()
        {
            var user1 = new TestUser
            {
                Id = "u002",
                Name = "Pawel",
                Rank = 1012,
                Address = new Address { Number = 1, Street = "abc"}
            };

            var user2 = new TestUser
            {
                Id = "u003",
                Name = "Adam",
                Rank = 1013,
                Address = new Address { Number = 2, Street = "def" }
            };

            users.InsertOne(user1); users.InsertOne(user2);

            PrintAllUsers("Insert result");

            user1.Name = "Olo";
            users.Update(user1);

            PrintAllUsers("Update result");

            users.Delete(user1.Id);

            PrintAllUsers("Delete result");

            var findTest = users.Find(u => u.Name == "Adam").FirstOrDefault();
            Assert.IsNotNull(findTest);
            Assert.AreEqual("u003", findTest.Id);

            var findTest2 = users.Find(u => u.Rank == 1013).FirstOrDefault();
            Assert.IsNotNull(findTest2);
            Assert.AreEqual("u003", findTest2.Id);
        }

        private void PrintAllUsers(string header)
        {
            Console.WriteLine(header);
            foreach (var user in users.FindAll())
                Console.WriteLine(user.ToString());
            Console.WriteLine();
        }
    }
}
