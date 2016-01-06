using System.Linq;
using ElasticLinq.Async;
using ElasticSearch.Linq.Mapping;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// Rappresents a query provider for connectiong with Elastic search system.
    /// </summary>
    public interface IElasticQueryProvider
        : IQueryProvider, IAsyncQueryExecutor
    {
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        IElasticConnection Connection { get; }

        /// <summary>
        /// Gets the mapping.
        /// </summary>
        /// <value>
        /// The mapping.
        /// </value>
        IElasticMapping Mapping { get; }
    }
}
