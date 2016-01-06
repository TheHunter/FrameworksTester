using ElasticSearch.Linq.Request.Criteria;
using Newtonsoft.Json;

namespace ElasticSearch.Linq.Request
{
    public class CriteriaFilter
    {
        [JsonProperty(PropertyName = "filtered")]
        public QueryFilter Filtered { get; set; }
    }

    public class QueryFilter
    {
        [JsonProperty(PropertyName = "filter")]
        public ICriteria Filter { get; set; }

        [JsonProperty(PropertyName = "query")]
        public ICriteria Query { get; set; }
    }
}