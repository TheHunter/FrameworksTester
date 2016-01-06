namespace ElasticSearch.Linq.Test.Model
{
    public class Person
    {
        protected Person()
        {
        }

        public Person(int id)
        {
            this.Id = id;
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Cf { get; set; }

        public long Version { get; set; }
    }
}
