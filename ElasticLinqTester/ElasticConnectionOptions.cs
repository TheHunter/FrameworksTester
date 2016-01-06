namespace ElasticSearch.Linq
{
    /// <summary>
    /// Connection options that can be specified to control how <see cref="ElasticSession"/> communicates with
    /// Elasticsearch.
    /// </summary>
    public class ElasticConnectionOptions
    {
        /// <summary>
        /// Gets whether the JSON should be prettified to make it more human-readable.
        /// </summary>
        /// <remarks>Defaults to false.</remarks>
        public bool Pretty { get; set; }

        /// <summary>
        /// Gets the default size for searches to specify the maximum document count.
        /// </summary>
        /// <remarks>Defaults to null, resulting in Elasticseach defaulting to 10.</remarks>
        public long? SearchSizeDefault { get; set; }
    }
}
