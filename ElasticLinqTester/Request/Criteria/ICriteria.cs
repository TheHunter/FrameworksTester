namespace ElasticSearch.Linq.Request.Criteria
{
    /// <summary>
    /// Interface that all criteria must implement to be part of
    /// the query filter tree.
    /// </summary>
    public interface ICriteria
    {
        /// <summary>
        /// Gets the name of this criteria as specified in the Elasticsearch DSL.
        /// </summary>
        string Name { get; }
    }
}
