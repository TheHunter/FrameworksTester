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
            this.typesToInspect = new HashSet<Type>(typesToInspect ?? Enumerable.Empty<Type>());
        }

        public override byte[] Serialize(object data, SerializationFormatting formatting = SerializationFormatting.Indented)
        {
            var format = formatting == SerializationFormatting.None ? Formatting.None : Formatting.Indented;
            
            if (data == null)
                return null;

            Type dataType = data.GetType();
            var ret = base.Serialize(data, formatting);

            if (dataType.IsGenericType)
                dataType = dataType.GetGenericTypeDefinition();

            if (this.typesToInspect.Contains(dataType))
            {
                string originalJson = Encoding.UTF8.GetString(ret);
                var jObject = JObject.Parse(originalJson);
                if (jObject.Remove("$type"))
                    return Encoding.UTF8.GetBytes(jObject.ToString(format));
            }

            return ret;
        }
    }


    //public class MuClassDev
    //{
    //    public string MyLoalprop { get; set; }
    //    public IMyCustomProperty MyInterfaceproperty { get; set; }
    //}

    //public interface IMyCustomProperty
    //{
    //    string Name { get; set; }
    //}

    //public class MyCustomProp1 : IMyCustomProperty
    //{
    //    public string Name { get; set; }
    //}

    //public class MyCustomProp2 : IMyCustomProperty
    //{
    //    public string Name { get; set; }
    //    public int Code { get; set; }
    //}
}
