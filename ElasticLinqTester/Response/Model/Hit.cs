using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Response.Model
{
    /// <summary>
    /// An individual hit response which rappresents any document from Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{Type} in {Index} id {Id}")]
    public class Hit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hit"/> class.
        /// </summary>
        public Hit()
        {
            this.Fields = new Dictionary<string, JToken>();
        }

        /// <summary>
        /// Gets or sets the index of the document responsible for this hit.
        /// </summary>
        //[JsonProperty(PropertyName = "_index", ItemConverterType = typeof(IConvertible))]
        [JsonProperty(PropertyName = "_index")]
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets the type of document used to create this hit.
        /// </summary>
        [JsonProperty(PropertyName = "_type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the Unique index of the document responsible for this hit.
        /// </summary>
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the version of this document.
        /// </summary>
        [JsonProperty(PropertyName = "_version")]
        public long? Version { get; set; }

        /// <summary>
        /// Gets or sets the score this hit achieved based on the query criteria.
        /// </summary>
        [JsonProperty(PropertyName = "_score")]
        public double? Score { get; set; }

        /// <summary>
        /// Gets or sets the highlighting for this hit if highlighting was requested.
        /// </summary>
        [JsonProperty(PropertyName = "highlight")]
        public JObject Highlight { get; set; }

        /// <summary>
        /// Gets or sets the actual document for this hit (not supplied if fields requested).
        /// </summary>
        [JsonProperty(PropertyName = "_source")]
        public JObject Source { get; set; }

        /// <summary>
        /// Gets or sets the list of fields for this hit extracted from the document (if fields requested).
        /// </summary>
        [JsonProperty(PropertyName = "fields")]
        public Dictionary<string, JToken> Fields { get; set; }
    }
}
