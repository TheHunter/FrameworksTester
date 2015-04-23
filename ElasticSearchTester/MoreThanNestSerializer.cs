using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elasticsearch.Net.Serialization;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearchTester
{
    public class MoreThanNestSerializer
        : NestSerializer
    {
        private readonly JsonSerializerSettings serializationSettings;
        private readonly HashSet<Type> typesToInspect;

        public MoreThanNestSerializer(IConnectionSettingsValues connectionSettings, IEnumerable<Type> typesToInspect = null)
            : base(connectionSettings)
        {
            //this.serializationSettings = this.CreateLocalSettings(connectionSettings);
            this.typesToInspect = new HashSet<Type>(typesToInspect ?? Enumerable.Empty<Type>());
        }

        public override byte[] Serialize(object data, SerializationFormatting formatting = SerializationFormatting.Indented)
        {
            var format = formatting == SerializationFormatting.None ? Formatting.None : Formatting.Indented;
            //JObject jo = JObject.FromObject(data, JsonSerializer.CreateDefault(this.serializationSettings));

            //jo.Remove("$type");

            //var serialized = jo.ToString(format);
            //return string.IsNullOrWhiteSpace(serialized) ? null : Encoding.UTF8.GetBytes(serialized);

            if (data == null)
                return null;

            Type t = data.GetType();
            var ret = base.Serialize(data, formatting);
            //if (this.typesToInspect.Contains(t))
            //{
            //    string originalJson = Encoding.UTF8.GetString(ret);
            //    var jObject = JObject.Parse(originalJson);
            //    if (jObject.Remove("$type"))
            //        return Encoding.UTF8.GetBytes(jObject.ToString(format));
            //}

            string originalJson = Encoding.UTF8.GetString(ret);
            var jObject = JObject.Parse(originalJson);
            if (jObject.Remove("$type"))
                return Encoding.UTF8.GetBytes(jObject.ToString(format));

            return ret;
        }
    }
}
