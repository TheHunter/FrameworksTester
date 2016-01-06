// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticSearch.Linq.Request.Criteria;

namespace ElasticSearch.Linq.Request.Facets
{
    /// <summary>
    /// Interface that all Facets must implement to be part of
    /// the query facet tree.
    /// </summary>
    public interface IFacet
    {
        /// <summary>
        /// Gets the name of this facet as specified in the Elasticsearch DSL.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of this facet as specified in the Elasticsearch DSL
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the criteria of this facet
        /// </summary>
        ICriteria Filter { get; }
    }

    /// <summary>
    /// Interface that all orderable Facets must implement to be part of
    /// the query facet tree.
    /// </summary>
    public interface IOrderableFacet : IFacet
    {
        /// <summary>
        /// Gets the dimension of top terms should be returned out of the overall terms list
        /// </summary>
        int? Size { get; }
    }
}