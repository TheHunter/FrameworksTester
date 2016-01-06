using System.Diagnostics;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq.Request.Facets
{
    /// <summary>
    /// Represents a filter facet.
    /// Filter Facets return the number of documents that  match the specified filter criteria.
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => 1).Select(a => a.Sum(b => b.Field))</remarks>
    [DebuggerDisplay("FilterFacet {Filter}")]
    public class FilterFacet : IFacet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterFacet"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="filter">The filter.</param>
        public FilterFacet(string name, ICriteria filter)
        {
            Argument.CheckNotEmpty("name", name);

            this.Name = name;
            this.Filter = filter;
        }

        /// <summary>
        /// Gets the type of this facet as specified in the Elasticsearch DSL
        /// </summary>
        public string Type
        {
            get { return "filter"; }
        }

        /// <summary>
        /// Gets the name of this facet as specified in the Elasticsearch DSL
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the criteria of this facet
        /// </summary>
        public ICriteria Filter { get; private set; }
    }
}