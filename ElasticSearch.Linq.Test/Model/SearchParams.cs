using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ElasticSearch.Linq.Converter;
using ElasticSearch.Linq.Request;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Request.Facets;
using Newtonsoft.Json;

namespace ElasticSearch.Linq.Test.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SearchParams
    {
        public SearchParams()
        {
            this.SortOptions = new List<SortOption>();
        }

        [JsonProperty(PropertyName = "fields", Order = 0)]
        public List<string> Fields { get; set; }

        public string DocumentType { get; set; }

        [JsonProperty(PropertyName = "min_score", Order = 2)]
        public double? MinScore { get; set; }

        [JsonProperty(PropertyName = "version", Order = 5)]
        public bool Version { get; set; }

        public ICriteria Filter { get; set; }

        public ICriteria Query { get; set; }

        [JsonProperty(PropertyName = "query", Order = 6)]
        protected dynamic QueryAdapter
        {
            get
            {
                if (this.Filter == null)
                    return this.Query;

                return new CriteriaFilter
                {
                    Filtered = new QueryFilter
                    {
                        Filter = this.Filter,
                        Query = this.Query
                    }
                };
            }
        }

        public string SearchType { get; set; }

        [JsonProperty(PropertyName = "sort", Order = 7, ItemConverterType = typeof(SortOptionConverter))]
        public List<SortOption> SortOptions { get; }

        [JsonProperty(PropertyName = "from", Order = 8)]
        public long @From { get; set; }

        [JsonProperty(PropertyName = "highlight", Order = 9)]
        public Highlight Highlight { get; set; }

        [JsonProperty(PropertyName = "size", Order = 10)]
        public long? Size { get; set; }
        
        [JsonConverter(typeof(DynamicFacetJsonConverter))]
        [JsonProperty(PropertyName = "Facets", Order = 12)]
        public List<IFacet> Facets { get; set; }

        public TimeSpan Timeout { get; set; }

        [JsonProperty(PropertyName = "timeout", Order = 14)]
        protected string TimeoutAtr
        {
            get
            {
                var transportCulture = CultureInfo.CurrentCulture;
                
                if (this.Timeout.Milliseconds != 0)
                    return this.Timeout.TotalMilliseconds.ToString(transportCulture);

                if (this.Timeout.Seconds != 0)
                    return this.Timeout.TotalSeconds.ToString(transportCulture) + "s";

                return this.Timeout.TotalMinutes.ToString(transportCulture) + "m";
            }
        }

        #region customization for serializing members

        public bool ShouldSerializeSortOptions()
        {
            return this.SortOptions != null && SortOptions.Any();
        }

        public bool ShouldSerializeSize()
        {
            return (this.Facets != null && !this.Facets.Any());
        }
        #endregion
    }
}
