using System.Linq;
using ElasticSearch.Linq.Request;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// Represents a LINQ query that will be sent to Elasticsearch.
    /// </summary>
    /// <typeparam name="T">Type of element to be queried.</typeparam>
    public interface IElasticQuery<out T> : IOrderedQueryable<T>
    {
        /// <summary>
        /// Returns the query information including the JSON payload and Uri.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryInfo"/>.
        /// </returns>
        QueryInfo ToQueryInfo();
    }
}
