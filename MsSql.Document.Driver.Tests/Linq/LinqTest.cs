using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MsSql.Document.Driver.Tests.TestModel;
using NUnit.Framework;

namespace MsSql.Document.Driver.Tests.Linq
{
    [TestFixture]
    public class LinqTest
    {
        private readonly Dictionary<string, string> indexes = new Dictionary<string, string>
        {
            {"Id", "Id"}
        };

        [Test]
        public void Equals()
        {
            Expression<Func<TestUser, bool>> predicate = u => u.Name == "Abc";
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE JSON_VALUE(json, '$.Name') = @param_0", cmd.CommandText);
            Assert.AreEqual("Abc", cmd.Parameters[0].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        [Ignore("NULL query is not implemented yet")]
        public void IsNull()
        {
            Expression<Func<TestUser, bool>> predicate = u => u.Name == null;
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE JSON_VALUE(json, '$.Name') = @param_0", cmd.CommandText);
            Assert.IsNull(cmd.Parameters[0].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        public void NotEquals()
        {
            Expression<Func<TestUser, bool>> predicate = u => u.Name != "Abc";
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE JSON_VALUE(json, '$.Name') <> @param_0", cmd.CommandText);
            Assert.AreEqual("Abc", cmd.Parameters[0].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        public void SimpleIndex()
        {
            Expression<Func<TestUser, bool>> predicate = u => u.Id == "u12";
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE Id = @param_0", cmd.CommandText);
            Assert.AreEqual("u12", cmd.Parameters[0].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        public void IndexAndNonIndex()
        {
            Expression<Func<TestUser, bool>> predicate = u => u.Id == "u12" && u.Name == "Abc";
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE (Id = @param_0) AND (JSON_VALUE(json, '$.Name') = @param_1)", cmd.CommandText);
            Assert.AreEqual("u12", cmd.Parameters[0].Value);
            Assert.AreEqual("Abc", cmd.Parameters[1].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        public void IndexOrNonIndex()
        {
            Expression<Func<TestUser, bool>> predicate = u => u.Id == "u12" || u.Name == "Abc";
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE (Id = @param_0) OR (JSON_VALUE(json, '$.Name') = @param_1)", cmd.CommandText);
            Assert.AreEqual("u12", cmd.Parameters[0].Value);
            Assert.AreEqual("Abc", cmd.Parameters[1].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        public void ManyBinaryOperators()
        {
            Expression<Func<TestUser, bool>> predicate = u => u.Id == "u12" || u.Name == "Abc" && u.Rank >= 5;
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE (Id = @param_0) OR ((JSON_VALUE(json, '$.Name') = @param_1) AND (JSON_VALUE(json, '$.Rank') >= @param_2))", cmd.CommandText);
            Assert.AreEqual("u12", cmd.Parameters[0].Value);
            Assert.AreEqual("Abc", cmd.Parameters[1].Value);
            Assert.AreEqual(5, cmd.Parameters[2].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        public void GrupedBinaryOperators()
        {
            Expression<Func<TestUser, bool>> predicate = u => (u.Id == "u12" || u.Name == "Abc") && (u.Id == "u13" || u.Name == "Def");
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE ((Id = @param_0) OR (JSON_VALUE(json, '$.Name') = @param_1)) AND ((Id = @param_2) OR (JSON_VALUE(json, '$.Name') = @param_3))", cmd.CommandText);
            Assert.AreEqual("u12", cmd.Parameters[0].Value);
            Assert.AreEqual("Abc", cmd.Parameters[1].Value);
            Assert.AreEqual("u13", cmd.Parameters[2].Value);
            Assert.AreEqual("Def", cmd.Parameters[3].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        public void GraterThan()
        {
            Expression<Func<TestUser, bool>> predicate = u => u.Rank >= 5;
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE JSON_VALUE(json, '$.Rank') >= @param_0", cmd.CommandText);
            Assert.AreEqual(5, cmd.Parameters[0].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        public void IndexUsage()
        {
            var ix = new Dictionary<string, string>
            {
                {"Id", "Id"},
                {"Address.Number", "IX-Address-Number"}
            };
            Expression<Func<TestUser, bool>> predicate = u => u.Address.Number >= 5;
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, ix);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE IX-Address-Number >= @param_0", cmd.CommandText);
            Assert.AreEqual(5, cmd.Parameters[0].Value);
            Console.WriteLine(cmd.CommandText);
        }

        [Test]
        public void ComplexMember()
        {
            Expression<Func<TestUser, bool>> predicate = u => u.Address.Number == 5;
            SqlLinqParser ec = new SqlLinqParser("TestUser", predicate, indexes);
            var cmd = ec.Parse();
            Assert.AreEqual("SELECT id, json FROM [TestUser] WHERE JSON_VALUE(json, '$.Address.Number') = @param_0", cmd.CommandText);
            Assert.AreEqual(5, cmd.Parameters[0].Value);
            Console.WriteLine(cmd.CommandText);
        }

    }
}
