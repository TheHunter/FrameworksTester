using System;
using System.Collections.Generic;
using System.Linq;
using ElasticSearch.Linq.Request.Facets;
using ElasticSearch.Linq.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Converter
{
    /// <summary>
    /// Converts an IFacet collection into dynamic object, injecting properties from collection items.
    /// </summary>
    public class DynamicFacetJsonConverter
        : JsonConverter
    {
        private static readonly Type ItemType = typeof(IFacet);
        private static readonly Type FacetConverType = typeof(IFacetConverter);
        private static readonly Type CriteriaConverType = typeof(ICriteriaConverter);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!this.CanConvert(value.GetType()))
                throw new InvalidOperationException("The given argument cannot be serialized because is not an IFacet collection type.");

            try
            {
                var converters = serializer.Converters;
                
                var facetConverter = converters
                    .FirstOrDefault(converter => FacetConverType.IsInstanceOfType(converter)) as IFacetConverter
                                     ?? new FacetJsonConverter();

                var criteriaConverter = converters
                    .FirstOrDefault(converter => CriteriaConverType.IsInstanceOfType(converter)) as ICriteriaConverter
                                        ?? new CriteriaJsonConverter();

                var result = new JObject();
                foreach (var item in value as dynamic)
                {
                    result.Add(item.Name, facetConverter.MakeToken(item, criteriaConverter));
                }

                using (var reader = new JTokenReader(result))
                {
                    writer.WriteToken(reader);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error on writting the given facet, see inner exception for details.", ex);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Method not invokable...");
        }

        public override bool CanConvert(Type objectType)
        {
            Type elementType;
            if (objectType.IsArray)
            {
                elementType = objectType.GetElementType();
            }
            else if (objectType.IsGenericType && objectType.Implements(typeof(IEnumerable<>)))
            {
                elementType = objectType.GetGenericArguments()[0];
            }
            else
            {
                return false;
            }
            return ItemType.IsAssignableFrom(elementType);
        }
    }
}
