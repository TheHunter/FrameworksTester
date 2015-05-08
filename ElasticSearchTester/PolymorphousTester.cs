using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using Elasticsearch.Net;
using ElasticSearchTester.Domain;
using ElasticSearchTester.Extensions;
using ElasticSearchTester.Json.Resolvers;
using Nest;
using Nest.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace ElasticSearchTester
{
    public class PolymorphousTester
    {
        [Fact]
        public void TestOnStudentPol()
        {
            var client = MakeElasticClient("polystudent");

            client.Delete<Student>(1);
            client.Delete<Student>(2);

            var instance0 = new Student
            {
                Id = 1,
                Name = "Name1",
                Size = 1,
                DataEncoded = "dlskfndlksfnkldsnfkl="
            };

            var instance1 = new StudentDev
            {
                Id = 2,
                Name = "Name2",
                Size = 2,
                DataEncoded = "dlskfndlksfnkldsnfkl=",
                University = "home"
            };

            var resp0 = client.Index<Student>(instance0);
            var resp1 = client.Index<Student>(instance1);

            Assert.True(resp0.Created);
            Assert.True(resp1.Created);
        }

        [Fact]
        public void TestOnReadStudentsPol()
        {
            var client = MakeElasticClient("polystudent");
            var response = client.Search<dynamic>(descriptor => descriptor
                .Type("student")
                .From(0)
                .Size(2));

            //var searchRequest = new SearchRequest("polystudent", "student")
            //{
            //    From = 0,
            //    Size = 10
            //};
            //var response = client.Search<Student>(searchRequest);

            var res0 = client.Get<Student>(1);
            Assert.True(res0.Found);

            var res1 = client.Get<Student>(2);
            Assert.True(res1.Found);

            Assert.NotNull(response);
            Assert.Equal(2, response.Documents.Count());
        }

        [Fact]
        public void TestNew()
        {
            var client = MakeElasticClient("studentnew");

            client.Delete<StudentTwo>(1);
            var instance = new StudentTwo
            {
                Id = 1,
                Name = "naming",
                Parameters = new List<GenParameter>
                {
                    new GenParameter{ Name = "par1", Value = 10 },
                    new GenParameter{ Name = "par1", Value = 52.36 },
                    //new GenParameter{ Name = "par1", Value = "my parameter" }
                }
            };
            var response = client.Index(instance);
            Assert.True(response.Created);
        }

        [Fact]
        public void TestNewEmployer()
        {
            var client = MakeElasticClient("employers");

            var instance = new Employer
            {
                Name = "naming",
                Salary = 25.50,
                Surname = "surnaming"
            };

            var id = client.Infer.Id(instance);
            Assert.Null(id);

            var resolver = new IdResolverNew();
            Assert.False(resolver.HasIdProperty<Employer>());

            Assert.True(resolver.HasIdProperty<Company>());
        }

        private static ElasticClient MakeElasticClient(string defaultIndex)
        {
            var list = new List<Type>
            {
                typeof(SearchDescriptor<>)
            };

            var settings = MakeSettings(defaultIndex)
                .ExposeRawResponse()
                .UsePrettyResponses()
                ;

            settings.SetJsonSerializerSettingsModifier(
                delegate(JsonSerializerSettings zz)
                {
                    zz.NullValueHandling = NullValueHandling.Ignore;
                    zz.MissingMemberHandling = MissingMemberHandling.Ignore;
                    zz.TypeNameHandling = TypeNameHandling.Objects;
                    zz.Binder = new CustomBinder(
                        new List<KeyValuePair<string, Type>>
                        {
                            new KeyValuePair<string, Type>("Student", typeof(Student)),
                            new KeyValuePair<string, Type>("StudentDev", typeof(StudentDev)),
                            new KeyValuePair<string, Type>("StudentNew", typeof(StudentDev))
                        });

                    zz.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                    zz.ContractResolver = new DynamicContractResolver(settings);
                });
            return new ElasticClient(settings, null, new MoreThanNestSerializer(settings, list));
        }

        private static ElasticClient MakeDefaultClient(string defaultIndex)
        {
            var list = new List<Type>
            {
                typeof(SearchDescriptor<>)
            };

            var settings = MakeSettings(defaultIndex);
            return new ElasticClient(settings, null, new MoreThanNestSerializer(settings, list));
        }

        private static ConnectionSettings MakeSettings(string defaultIndex)
        {
            var uri = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(uri, defaultIndex);
            return settings;
        }

    }

    public class IdResolverNew : IdResolver
    {
        public bool HasIdProperty<TEntity>()
        {
            return this.HasIdProperty(typeof(TEntity));
        }

        public bool HasIdProperty(Type type)
        {
            var prop = this.IdInfo(type);
            return prop != null;
        }

        public PropertyInfo IdInfo<TEntity>()
        {
            return this.IdInfo(typeof(TEntity));
        }

        public PropertyInfo IdInfo(Type type)
        {
            ElasticTypeAttribute elasticTypeAttribute = ElasticAttributes.Type(type);
            if (elasticTypeAttribute != null && !string.IsNullOrWhiteSpace(elasticTypeAttribute.IdProperty))
                return this.GetPropertyCaseInsensitive(type, elasticTypeAttribute.IdProperty);
            string propertyName = "IdSession";
            PropertyInfo propertyCaseInsensitive1 = this.GetPropertyCaseInsensitive(type, propertyName);
            if (propertyCaseInsensitive1 != (PropertyInfo)null)
                return propertyCaseInsensitive1;
            PropertyInfo propertyCaseInsensitive2 = this.GetPropertyCaseInsensitive(type, type.Name + propertyName);
            if (propertyCaseInsensitive2 != (PropertyInfo)null)
                return propertyCaseInsensitive2;
            PropertyInfo propertyCaseInsensitive3 = this.GetPropertyCaseInsensitive(type, type.Name + "_" + propertyName);
            if (propertyCaseInsensitive3 != (PropertyInfo)null)
                return propertyCaseInsensitive3;
            else
                return propertyCaseInsensitive3;
        }

        private PropertyInfo GetPropertyCaseInsensitive(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
        }
    }
}
