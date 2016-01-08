using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearch.Linq.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Converter
{
    public class SortOptionConverter
        : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                var sortOption = value as SortOption;
                if (sortOption != null)
                {
                    var result = Build(sortOption);

                    using (var reader = new JTokenReader(result))
                    {
                        writer.WriteToken(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error when instance to serialize was executed.", "value", ex);
            }
        }

        private static JToken Build(SortOption sortOption)
        {
            if (!sortOption.IgnoreUnmapped)
                return sortOption.Ascending
                    ?  new JValue(sortOption.Name) as JToken     //(object)sortOption.Name
                    : new JObject(new JProperty(sortOption.Name, "desc"));

            var properties = new List<JProperty> { new JProperty("ignore_unmapped", true) };
            if (!sortOption.Ascending)
                properties.Add(new JProperty("order", "desc"));

            return new JObject(new JProperty(sortOption.Name, new JObject(properties)));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(SortOption) == objectType;
        }
    }
}
