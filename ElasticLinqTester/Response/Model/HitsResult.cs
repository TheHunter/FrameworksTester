using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ElasticSearch.Linq.Response.Model
{
    /// <summary>
    /// Rappresents a collection of hits with additional info from Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{Hits.Count} hits of {Total}")]
    public class HitsResult
    {
        /// <summary>
        /// Gets or sets the total number of Hits available on the server.
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public long Total { get; set; }

        /// <summary>
        /// Gets or sets the highest score of a hit for the given query.
        /// </summary>
        [JsonProperty(PropertyName = "max_score")]
        public double? MaxScore { get; set; }

        /// <summary>
        /// Gets or sets the list of Hits received from the server.
        /// </summary>
        [JsonProperty(PropertyName = "hits")]
        public List<Hit> Hits { get; set; }
    }
}
