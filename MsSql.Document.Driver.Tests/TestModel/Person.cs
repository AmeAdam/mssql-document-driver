namespace MsSql.Document.Driver.Tests.TestModel
{
    public class Person : IIdentifiable
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
}