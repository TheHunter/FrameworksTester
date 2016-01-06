using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Response.Model
{
    /// <summary>
    /// Rappresents the response structure from Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{Hits.Hits.Count} hits in {Took} ms")]
    public class ElasticResponse
    {
        /// <summary>
        /// Gets or sets tow long the request took in milliseconds.
        /// </summary>
        [JsonProperty(PropertyName = "took")]
        public int Took { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this request timed out or not.
        /// </summary>
        [JsonProperty(PropertyName = "timed_out")]
        public bool TimedOut { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the search Hits delivered in this response.
        /// </summary>
        [JsonProperty(PropertyName = "hits")]
        public HitsResult Hits { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the error received from Elasticsearch.
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public JValue Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the Facets delivered in this response.
        /// </summary>
        [JsonProperty(PropertyName = "Facets")]
        public JObject Facets { get; set; }
    }
}
