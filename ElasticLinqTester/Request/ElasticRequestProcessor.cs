using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticSearch.Linq.Mapping;
using ElasticSearch.Linq.Request.Formatters;
using ElasticSearch.Linq.Response.Model;
using ElasticSearch.Linq.Retry;
using ElasticSearch.Linq.Utility;
using Newtonsoft.Json;

namespace ElasticSearch.Linq.Request
{
    /// <summary>
    /// Sends Elasticsearch requests via HTTP and ensures materialization of the response.
    /// </summary>
    public class ElasticRequestProcessor
    {
        readonly IElasticConnection connection;
        readonly IElasticMapping mapping;
        readonly IRetryPolicy retryPolicy;

        public ElasticRequestProcessor(IElasticConnection connection, IElasticMapping mapping, IRetryPolicy retryPolicy)
        {
            Argument.CheckNotNull("connection", connection);
            Argument.CheckNotNull("mapping", mapping);
            Argument.CheckNotNull("retryPolicy", retryPolicy);

            this.connection = connection;
            this.mapping = mapping;
            this.retryPolicy = retryPolicy;
        }

        public Task<ElasticResponse> SearchAsync(SearchRequest searchRequest, CancellationToken cancellationToken)
        {
            var formatter = new SearchRequestFormatter(this.connection, this.mapping, searchRequest);

            return this.retryPolicy.ExecuteAsync(
                async token => await this.connection.SearchAsync(
                    formatter.Body,
                    searchRequest,
                    token),

                (response, exception) => !cancellationToken.IsCancellationRequested && exception != null,
                (response, additionalInfo) =>
                {
                    additionalInfo["index"] = connection.Index;
                    additionalInfo["uri"] = connection.GetSearchUri(searchRequest);
                    additionalInfo["query"] = formatter.Body;
                }, cancellationToken);
        }

        async Task<HttpResponseMessage> SendRequestAsync(HttpClient httpClient, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await httpClient.SendAsync(requestMessage, cancellationToken);
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

        static IEnumerable<string> GetResultSummary(ElasticResponse results)
        {
            if (results == null)
            {
                yield return "nothing";
            }
            else
            {
                if (results.Hits != null && results.Hits.Hits != null && results.Hits.Hits.Count > 0)
                    yield return results.Hits.Hits.Count + " Hits";

                if (results.Facets != null && results.Facets.Count > 0)
                    yield return results.Facets.Count + " Facets";
            }
        }
    }
}
