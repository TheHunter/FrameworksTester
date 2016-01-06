using System.Linq;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// Represents a unit of work in ElasticSearch.Linq.
    /// </summary>
    public interface IElasticSession
    {
        /// <summary>
        /// Gets a query that can search for documents of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <returns>The query that can search for documents of the given type.</returns>
        IQueryable<T> Query<T>();

        /// <summary>
        /// Gets a query that can search for documents of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">document type.</typeparam>
        /// <param name="index">The index.</param>
        /// <returns>The query which can used to search documents of the given type.</returns>
        IQueryable<T> Query<T>(string index);

        /// <summary>
        /// Gets a query that can search for documents of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">document type</typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>The query which can used to search documents of the given type.</returns>
        IQueryable<T> Query<T>(IElasticConnection connection);
    }
}
