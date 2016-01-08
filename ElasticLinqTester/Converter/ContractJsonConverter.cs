using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ElasticSearch.Linq.Converter
{
    /// <summary>
    /// A custom resolver for serializing instances which must be converted into another type through any contract resolver which
    /// implements IInstanceResolver interface.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class ContractJsonConverter
        : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // IInstanceResolver creator = serializer.ContractResolver as IInstanceResolver;
            // var instance = creator.Resolve(value);
            // The line above must throw an exception if type value is not managed...

            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
