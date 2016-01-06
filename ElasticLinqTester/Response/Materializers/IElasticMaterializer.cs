using ElasticSearch.Linq.Response.Model;

namespace ElasticSearch.Linq.Response.Materializers
{
    /// <summary>
    /// Interface for all materializers responsible for turning the ElasticResponse into desired
    /// CLR objects.
    /// </summary>
    public interface IElasticMaterializer
    {
        /// <summary>
        /// Materialize the ElasticResponse into the desired CLR objects.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> received from Elasticsearch.</param>
        /// <returns>List or a single CLR object as requested.</returns>
        object Materialize(ElasticResponse response);
    }
}