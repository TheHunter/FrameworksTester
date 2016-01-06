using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using ElasticSearch.Linq.Request.Facets;
using ElasticSearch.Linq.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Converter
{
    public class FacetJsonConverter
        : JsonConverter, IFacetConverter
    {
        private static readonly Type FacetType = typeof(IFacet);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!this.CanConvert(value.GetType()))
                throw new InvalidOperationException("The given argument cannot be serialized because is not a IFacet type.");

            try
            {
                var converter = serializer.Converters.FirstOrDefault(c => c.GetType() == typeof(CriteriaJsonConverter)) as CriteriaJsonConverter;
                var result = this.MakeToken(value as IFacet, converter);
                using (var reader = new JTokenReader(result))
                {
                    writer.WriteToken(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on writting the given facet, see inner exception for details.", ex);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Method not invokable...");
        }

        public override bool CanConvert(Type objectType)
        {
            return FacetType.IsAssignableFrom(objectType);
        }

        public virtual JObject MakeToken(IFacet facet, ICriteriaConverter criteriaConverter = null, int? defaultSize = 10)
        {
            try
            {
                Argument.CheckNotNull("facet", facet);

                var specificBody = this.Build(facet as dynamic);
                var orderableFacet = facet as IOrderableFacet;
                if (orderableFacet != null)
                {
                    var facetSize = orderableFacet.Size ?? defaultSize;
                    if (facetSize.HasValue)
                        specificBody["size"] = facetSize.Value.ToString(CultureInfo.CurrentCulture);
                }

                var namedBody = new JObject(new JProperty(facet.Type, specificBody));

                if (facet.Filter != null)
                {
                    // namedBody["filter"] = this.Build(facet.Filter as dynamic);
                    namedBody["filter"] = (criteriaConverter ?? new CriteriaJsonConverter())
                        .MakeToken(facet.Filter);
                }

                return namedBody;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error on translate the given facet into jproperty, see inner exception for details.", ex);
            }
        }

        #region
        protected JToken Build(IFacet facet)
        {
            if (facet == null)
                throw new ArgumentNullException("facet", "Facet must be referenced.");

            throw new NotImplementedException(string.Format("The given facet is not managed by this builder, type: {0}", facet.GetType().FullName));
        }

        protected JToken Build(StatisticalFacet facet)
        {
            return new JObject(
                BuildFieldProperty(facet.Fields)
            );
        }

        protected JToken Build(TermsStatsFacet facet)
        {
            return new JObject(
                new JProperty("key_field", facet.Key),
                new JProperty("value_field", facet.Value)
            );
        }

        protected JToken Build(TermsFacet facet)
        {
            return new JObject(BuildFieldProperty(facet.Fields));
        }

        protected JToken Build(FilterFacet facet)
        {
            return new JObject();
        }

        private static JToken BuildFieldProperty(ReadOnlyCollection<string> fields)
        {
            return fields.Count == 1
                ? new JProperty("field", fields.First())
                : new JProperty("fields", new JArray(fields));
        }
        #endregion
    }
}
