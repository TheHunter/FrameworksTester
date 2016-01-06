using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticSearch.Linq.Request;
using ElasticSearch.Linq.Response.Model;
using ElasticSearch.Linq.Utility;
using Newtonsoft.Json;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// Specifies connection parameters for Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{Endpoint.ToString(),nq}{Index,nq}")]
    public class ElasticConnectionImpl : ElasticConnectionBase, IDisposable
    {
        private readonly string[] parameterSeparator = { "&" };

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticConnectionImpl"/> class. 
        /// Create a new ElasticConnectionImpl with the given parameters defining its properties.
        /// </summary>
        /// <param name="endpoint">
        /// The URL endpoint of the Elasticsearch server.
        /// </param>
        /// <param name="userName">
        /// UserName to use to connect to the server (optional).
        /// </param>
        /// <param name="password">
        /// Password to use to connect to the server (optional).
        /// </param>
        /// <param name="timeout">
        /// TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).
        /// </param>
        /// <param name="index">
        /// Name of the index to use on the server (optional).
        /// </param>
        /// <param name="options">
        /// Additional options that specify how this connection should behave.
        /// </param>
        public ElasticConnectionImpl(Uri endpoint, string userName = null, string password = null, TimeSpan? timeout = null,
            string index = null, ElasticConnectionOptions options = null)
            : this(new HttpClientHandler(), endpoint, userName, password, index, timeout, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticConnectionImpl"/> class. 
        /// Create a new ElasticConnectionImpl with the given parameters for internal testing.
        /// </summary>
        /// <param name="innerMessageHandler">
        /// The HttpMessageHandler used to intercept network requests for testing.
        /// </param>
        /// <param name="endpoint">
        /// The URL endpoint of the Elasticsearch server.
        /// </param>
        /// <param name="userName">
        /// UserName to use to connect to the server (optional).
        /// </param>
        /// <param name="password">
        /// Password to use to connect to the server (optional).
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
        internal ElasticConnectionImpl(HttpMessageHandler innerMessageHandler, Uri endpoint, string userName = null, string password = null, string index = null, TimeSpan? timeout = null, ElasticConnectionOptions options = null)
            : base(endpoint, index, timeout, options)
        {
            var httpClientHandler = innerMessageHandler as HttpClientHandler;
            if (httpClientHandler != null && httpClientHandler.SupportsAutomaticDecompression)
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;

            HttpClient = new HttpClient(new ForcedAuthHandler(userName, password, innerMessageHandler), true);
        }

        /// <summary>
        /// Gets the HttpClient used for issuing HTTP network requests.
        /// </summary>
        internal HttpClient HttpClient { get; private set; }

        /// <summary>
        /// Dispose of this ElasticConnectionImpl and any associated resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (HttpClient != null)
                {
                    HttpClient.Dispose();
                    HttpClient = null;
                }
            }
        }

        /// <inheritdoc/>
        public override async Task<ElasticResponse> SearchAsync(
            string body,
            SearchRequest searchRequest,
            CancellationToken token
            )
        {
            var uri = this.GetSearchUri(searchRequest);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new StringContent(body) })
            using (var response = await this.SendRequestAsync(requestMessage, token))
            using (var responseStream = await response.Content.ReadAsStreamAsync())
                return ParseResponse(responseStream);
        }

        /// <inheritdoc/>
        public override Uri GetSearchUri(SearchRequest searchRequest)
        {
            var builder = new UriBuilder(this.Endpoint);
            builder.Path += (this.Index ?? "_all") + "/";

            if (!string.IsNullOrEmpty(searchRequest.DocumentType))
                builder.Path += searchRequest.DocumentType + "/";

            builder.Path += "_search";

            var parameters = builder.Uri.GetComponents(UriComponents.Query, UriFormat.Unescaped)
                .Split(this.parameterSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('='))
                .ToDictionary(k => k[0], v => v.Length > 1 ? v[1] : null);

            if (!string.IsNullOrEmpty(searchRequest.SearchType))
                parameters["search_type"] = searchRequest.SearchType;

            if (Options.Pretty)
                parameters["pretty"] = "true";

            builder.Query = string.Join("&", parameters.Select(p => p.Value == null ? p.Key : p.Key + "=" + p.Value));

            return builder.Uri;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage requestMessage, CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await HttpClient.SendAsync(requestMessage, token);
            stopwatch.Stop();

            response.EnsureSuccessStatusCode();
            return response;
        }

        internal static ElasticResponse ParseResponse(Stream responseStream)
        {
            var stopwatch = Stopwatch.StartNew();

            using (var textReader = new JsonTextReader(new StreamReader(responseStream)))
            {
                var results = new JsonSerializer().Deserialize<ElasticResponse>(textReader);
                stopwatch.Stop();

                // var resultSummary = string.Join(", ", GetResultSummary(results));

                return results;
            }
        }

        internal static IEnumerable<string> GetResultSummary(ElasticResponse results)
        {
            if (results == null)
            {
                yield return "nothing";
            }
            else
            {
                if (results.Hits != null && results.Hits.Hits != null && results.Hits.Hits.Count > 0)
                    yield return results.Hits.Hits.Count + " hits";

                if (results.Facets != null && results.Facets.Count > 0)
                    yield return results.Facets.Count + " Facets";
            }
        }
    }
}
