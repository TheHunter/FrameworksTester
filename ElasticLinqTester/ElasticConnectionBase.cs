using System;
using System.Threading;
using System.Threading.Tasks;
using ElasticSearch.Linq.Request;
using ElasticSearch.Linq.Response.Model;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// Specifies connection parameters for Elasticsearch.
    /// </summary>
    public abstract class ElasticConnectionBase
        : IElasticConnection
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticConnectionBase"/> class. 
        /// Create a new ElasticConnectionBase with the given parameters for internal testing.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="index">
        /// Name of the index to use on the server (optional).
        /// </param>
        /// <param name="timeout">
        /// TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).
        /// </param>
        /// <param name="options">
        /// Additional options that specify how this connection should behave.
        /// </param>
        protected ElasticConnectionBase(Uri endpoint, string index = null, TimeSpan? timeout = null, ElasticConnectionOptions options = null)
        {
            if (timeout.HasValue)
                Argument.CheckPositive("value", timeout.Value);
            if (index != null)
                Argument.CheckNotEmpty("index", index);

            Argument.CheckNotNull("endpoint", endpoint);

            this.Endpoint = endpoint;
            this.Index = index;
            this.Options = options ?? new ElasticConnectionOptions();
            this.Timeout = timeout ?? DefaultTimeout;
        }

        /// <summary>
        /// Gets the name of the index on the Elasticsearch server.
        /// </summary>
        /// <example>northwind</example>
        public string Index { get; private set; }

        /// <summary>
        /// Gets how long to wait for a response to a network request before
        /// giving up.
        /// </summary>
        public TimeSpan Timeout { get; private set; }

        /// <summary>
        /// Gets additional options that specify how this connection should behave.
        /// </summary>
        public ElasticConnectionOptions Options { get; private set; }

        /// <summary>
        /// Gets the Uri that specifies the public endpoint for the server.
        /// </summary>
        /// <example>http://myserver.example.com:9200</example>
        public Uri Endpoint { get; private set; }

        /// <summary>
        /// Issues search requests to elastic search
        /// </summary>
        /// <param name="body">
        /// The request body
        /// </param>
        /// <param name="searchRequest">
        /// The search request settings
        /// </param>
        /// <param name="token">
        /// The cancellation token to allow aborting the operation
        /// </param>
        /// <returns>
        /// An elastic response
        /// </returns>
        public abstract Task<ElasticResponse> SearchAsync(string body, SearchRequest searchRequest, CancellationToken token);

        /// <summary>
        /// Gets the uri of the search
        /// </summary>
        /// <param name="searchRequest">The search request settings</param>
        /// <returns>The uri of the search</returns>
        public abstract Uri GetSearchUri(SearchRequest searchRequest);
    }
}
