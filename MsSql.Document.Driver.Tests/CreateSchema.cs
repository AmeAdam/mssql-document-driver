using System;
using System.Linq;
using NUnit.Framework;

namespace MsSql.Document.Driver.Tests
{
    [TestFixture]
    public class CreateSchema
    {
        private DocumentDatabase db;
        private DocumentCollection<TestUser> users;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            db = new DatabaseClient(".\\AME").GetDatabase("ame-projects");
            users = db.GetCollection<TestUser>("users");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            users.Drop();
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

            //var findTest3 = users.Find(u => u.Address.Number == 2).FirstOrDefault();
            //Assert.IsNotNull(findTest3);
            //Assert.AreEqual("u003", findTest3.Id);
        }

        private void PrintAllUsers(string header)
        {
            Console.WriteLine(header);
            foreach (var user in users.FindAll())
                Console.WriteLine(user.ToString());
            Console.WriteLine();
        }
    }

    public class TestUser : IIdentifiable
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Rank { get; set; }

        public Address Address { get; set; }

        public override string ToString()
        {
            return $"{Id} {Name} {Rank}";
        }
    }

    public class Address
    {
        public string Street { get; set; }

        public int Number { get; set; }

    }
}
