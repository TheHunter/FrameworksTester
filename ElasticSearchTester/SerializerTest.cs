using System;
using System.Collections.Generic;
using System.Globalization;
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
        public void TestOnSerializing()
        {
            DateTime data = new DateTime(2015, 5, 10);
            var format = new DateTimeFormatInfo();
            
            var des = data.ToString(format);
            Assert.NotNull(des);

            DateTime tt1;
            DateTime.TryParse(des, format, DateTimeStyles.None, out tt1);

            DateTime tt2;
            DateTime.TryParse(des, format, DateTimeStyles.None, out tt2);

            Assert.Equal(tt1.Day, tt2.Day);
            Assert.Equal(tt1.Month, tt2.Month);
            Assert.Equal(tt1.Year, tt2.Year);


            double d = 45.56;
            var aa = d.ToString(CultureInfo.GetCultureInfo("es-ES"));
            Assert.NotNull(aa);


            DateTime now = DateTime.Now;
            string rap = now.ToString(CultureInfo.InvariantCulture);
            string rap2 = now.ToString(new DateTimeFormatInfo());
            
            Assert.Equal(rap, rap2);
            Assert.NotNull(rap);
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
