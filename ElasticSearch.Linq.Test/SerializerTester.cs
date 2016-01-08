using System;
using System.Collections.Generic;
using ElasticSearch.Linq.Converter;
using ElasticSearch.Linq.Mapping;
using ElasticSearch.Linq.Request;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Request.Facets;
using ElasticSearch.Linq.Request.Formatters;
using ElasticSearch.Linq.Test.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ElasticSearch.Linq.Test
{
    public class SerializerTester
    {
        [Fact]
        public void TestSerializeOnPoco()
        {
            var instance = new SearchParams
            {
                Size = 10,
                Query = new QueryStringCriteria("ciao", new[] {"name", "surname"})
            };

            var settings = this.GetSettings();

            var serializer = JsonSerializer.Create(settings);

            var token = JToken.FromObject(instance, serializer);
            Assert.NotNull(token);

            var token2 = JToken.FromObject(instance.Query, serializer);
            Assert.NotNull(token2);

            var res = JsonConvert.SerializeObject(instance.Query, settings);
            Assert.NotNull(res);
        }

        [Fact]
        public void TestOnSerializePocos()
        {
            var instance0 = new SearchParams();
            var instance1 = new SearchRequest();

            this.InitParameters(instance0, instance1);
            var settings = this.GetSettings();

            var res0 = JsonConvert.SerializeObject(instance0, settings);
            var res1 = this.GetBodyExample(instance1);

            // inserire l'ordine di serializzazione..
            Assert.Equal(res0, res1);
        }

        public void InitParameters(SearchParams search1, SearchRequest search2)
        {
            search1.Fields = new List<string> { "name", "surname", "year" };
            search1.DocumentType = "person";
            search1.MinScore = 5.2;
            search1.Version = true;
            search1.Filter = new ExistsCriteria("name");
            search1.Query = new QueryStringCriteria("ciao", new[] { "name", "surname" });
            search1.SearchType = "nulltype";
            // search1.SortOptions = new List<SortOption>();
            //search1.SortOptions = new List<SortOption> { new SortOption("name", true), new SortOption("surname", false) };
            search1.From = 15;
            // search1.Highlight = new Highlight { PostTag = "posttag", PreTag = "pretag" };
            // search1.Highlight.AddFields("field1", "field2");
            search1.Size = 15;
            search1.Timeout = TimeSpan.FromSeconds(10);
            search1.Facets = new List<IFacet>
            {
                new FilterFacet("counter", new ExistsCriteria("surname")),
                new TermsFacet("name", 12, "field1", "field2"),
                new StatisticalFacet("countdown", "field4", "field5")
            };

            search2.Fields = search1.Fields;
            search2.DocumentType = search1.DocumentType;
            search2.MinScore = search1.MinScore;
            search2.Filter = search1.Filter;
            search2.Query = search1.Query;
            search2.SearchType = search1.SearchType;

            //search2.SortOptions = search1.SortOptions ?? new List<SortOption>();
            search2.SortOptions = search1.SortOptions;

            search2.From = search1.From;
            search2.Highlight = search1.Highlight;
            search2.Size = search1.Size;
            search2.Facets = search1.Facets;
        }

        public string GetBodyExample(SearchRequest instance)
        {
            var settings = this.GetSettings();

            var connection = new ElasticConnectionImpl(new Uri("http://localhost:9200"), index: "current,currentforfind");
            var mapping = new VeryElasticMapping(settings);

            var formatter = new SearchRequestFormatter(connection, mapping, instance);
            return formatter.Body;
        }

        public JsonSerializerSettings GetSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new VeryElasticContractResolver()
            };

            settings.Converters.Add(new CriteriaJsonConverter());
            return settings;
        }
    }
}
