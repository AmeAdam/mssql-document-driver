using System;
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
                Rank = 1012
            };

            var user2 = new TestUser
            {
                Id = "u003",
                Name = "Adam",
                Rank = 1013
            };

            users.InsertOne(user1); users.InsertOne(user2);

            PrintAllUsers("Insert result");

            user1.Name = "Olo";
            users.Update(user1);

            PrintAllUsers("Update result");

            users.Delete(user1.Id);

            PrintAllUsers("Delete result");
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

        public override string ToString()
        {
            return $"{Id} {Name} {Rank}";
        }
    }
}
