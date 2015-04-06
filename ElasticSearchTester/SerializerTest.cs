using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using ElasticSearchTester.Domain;
using ElasticSearchTester.Json.Resolvers;
using Nest;
using Newtonsoft.Json;
using Xunit;

namespace ElasticSearchTester
{
    public class SerializerTest
    {
        private IIdentity callerCredentials;
        private string user;

 
        public SerializerTest()
        {
            this.callerCredentials = WindowsIdentity.GetCurrent();
            this.user = this.callerCredentials == null ? "No windows identity" : this.callerCredentials.Name;
        }

        [Fact]
        public void TestSerializingDynamicInstance()
        {
            var settings = MakeSettings("attachment-repo");

            FileInfo info = new FileInfo(@"C:\Users\larman\Desktop\Test_ElasticSearch\source\Test1.txt");
            Attachment attachment = new Attachment(info, this.user);
            dynamic pointer = attachment;

            pointer.AnotherProp = "dynamic field";


            string json =
            JsonConvert.SerializeObject(
                attachment,
                Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new DynamicContractResolver(settings) }
                );

            Console.WriteLine(json);
        }

        private static ConnectionSettings MakeSettings(string defaultIndex)
        {
            var uri = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(uri, defaultIndex);
            return settings;
        }

    }
}
