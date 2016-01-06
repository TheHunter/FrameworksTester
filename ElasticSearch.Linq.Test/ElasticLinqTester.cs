using System;
using System.Linq;
using ElasticSearch.Linq.Mapping;
using ElasticSearch.Linq.Test.Model;
using Newtonsoft.Json;
using Xunit;

namespace ElasticSearch.Linq.Test
{
    public class ElasticLinqTester
    {
        [Theory]
        [InlineData("http://localhost:9200")]
        public void FirstTest(string url)
        {
            var connection = new ElasticConnectionImpl(new Uri(url), index: "current");

            var context = new ElasticSession(connection, new ElasticMapping(pluralizeTypeNames: false));

            var query = context.Query<Person>();
            Assert.NotNull(query);

            var res = query.ToList();
            Assert.NotEmpty(res);
        }

        [Theory]
        [InlineData("http://localhost:9200")]
        public void TestWithVeryElasticMapping(string url)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                ,ContractResolver = new VeryElasticContractResolver()
            };

            var connection = new ElasticConnectionImpl(new Uri(url), index: "current,currentforfind");

            var context = new ElasticSession(connection, new VeryElasticMapping(jsonSettings));

            var query = context.Query<Person>();
            Assert.NotNull(query);

            var res = query.ToList();
            Assert.NotEmpty(res);

            var q2 = context.Query<Person>("current").Where(person => person.Id > 3);
            var res2 = q2.ToList();
            Assert.NotEmpty(res2);
        }
    }
}
