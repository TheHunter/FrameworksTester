using System;
using System.Linq;
using ElasticSearch.Linq.Mapping;
using ElasticSearch.Linq.Retry;
using Newtonsoft.Json;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// Provides an entry point to easily create LINQ queries for Elasticsearch.
    /// </summary>
    public class ElasticSession : IElasticSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticSession"/> class.
        /// </summary>
        /// <param name="connection">The information on how to connect to the Elasticsearch server.</param>
        /// <param name="mapping">The object that helps map queries (optional, defaults to <see cref="VeryElasticMapping"/>).</param>
        /// <param name="retryPolicy">The object which controls retry policy for the search (optional, defaults to <see cref="RetryPolicy"/>).</param>
        public ElasticSession(IElasticConnection connection, IElasticMapping mapping = null, IRetryPolicy retryPolicy = null)
        {
            this.Connection = connection;
            this.Mapping = mapping ?? new VeryElasticMapping(
                new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }
                );
            this.RetryPolicy = retryPolicy ?? new RetryPolicy();
        }

        /// <summary>
        /// Gets the default connection to the Elasticsearch server.
        /// </summary>
        public IElasticConnection Connection { get; private set; }

        /// <summary>
        /// Gets the mapping to describe how objects and their properties are mapped to Elasticsearch.
        /// </summary>
        public IElasticMapping Mapping { get; private set; }

        /// <summary>
        /// Gets the retry policy for handling networking issues.
        /// </summary>
        public IRetryPolicy RetryPolicy { get; private set; }

        /// <summary>
        /// Gets a query that can search for documents of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <returns>
        /// The query that can search for documents of the given type.
        /// </returns>
        public virtual IQueryable<T> Query<T>()
        {
            return new VeryElasticQuery<T, VeryElasticQueryProvider>(new VeryElasticQueryProvider(this.Connection, this.Mapping, this.RetryPolicy));
        }

        /// <summary>
        /// Gets a query that can search for documents of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">document type.</typeparam>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The query which can used to search documents of the given type.
        /// </returns>
        public IQueryable<T> Query<T>(string index)
        {
            var connection = new ElasticConnectionImpl(new Uri(this.Connection.Endpoint.AbsoluteUri), index: index);
            return new VeryElasticQuery<T, VeryElasticQueryProvider>(new VeryElasticQueryProvider(connection, this.Mapping, this.RetryPolicy));
        }

        /// <summary>
        /// Gets a query that can search for documents of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">document type</typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>
        /// The query which can used to search documents of the given type.
        /// </returns>
        public IQueryable<T> Query<T>(IElasticConnection connection)
        {
            return new VeryElasticQuery<T, VeryElasticQueryProvider>(new VeryElasticQueryProvider(connection, this.Mapping, this.RetryPolicy));
        }
    }
}
