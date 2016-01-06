using System;
using System.Threading;
using System.Threading.Tasks;
using ElasticSearch.Linq.Request;
using ElasticSearch.Linq.Response.Model;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// The interface all clients which make requests to elastic search must implement
    /// </summary>
    public interface IElasticConnection
    {
        /// <summary>
        /// Gets the name of the index on the Elasticsearch server.
        /// </summary>
        string Index { get; }

        /// <summary>
        /// Gets additional options that specify how this connection should behave.
        /// </summary>
        ElasticConnectionOptions Options { get; }

        /// <summary>
        /// Gets how long to wait for a response to a network request before
        /// giving up.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Gets the Uri that specifies the public endpoint for the server.
        /// </summary>
        /// <example>http://mymachineserver.example.com:9200</example>
        /// <value>
        /// The endpoint.
        /// </value>
        Uri Endpoint { get; }

        /// <summary>
        /// Gets issues search requests to elastic search
        /// </summary>
        /// <param name="body">
        /// The request body
        /// </param>
        /// <param name="searchRequest">
        /// The search request settings
        /// </param>
        /// <param name="token">
        /// the token used to cancel operation.
        /// </param>
        /// <returns>
        /// An elastic response
        /// </returns>
        Task<ElasticResponse> SearchAsync(
            string body,
            SearchRequest searchRequest,
            CancellationToken token
            );

        /// <summary>
        /// Gets the uri of the search
        /// </summary>
        /// <param name="searchRequest">The search request settings</param>
        /// <returns>The uri of the search</returns>
        Uri GetSearchUri(SearchRequest searchRequest);
    }
}
