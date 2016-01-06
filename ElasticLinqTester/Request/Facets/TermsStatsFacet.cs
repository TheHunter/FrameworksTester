using System.Diagnostics;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq.Request.Facets
{
    /// <summary>
    /// Represents a terms_stats facet.
    /// Terms_stats Facets return all statistical information for
    /// a given field broken down by a term. 
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => a.Term).Select(a => a.Sum(b => b.Field))</remarks>
    [DebuggerDisplay("TermsStatsFacet \"{key,nq}.{value,nq}\"")]
    public class TermsStatsFacet : IOrderableFacet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermsStatsFacet"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="size">The size.</param>
        public TermsStatsFacet(string name, string key, string value, int? size)
            : this(name, null, key, value)
        {
            this.Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermsStatsFacet"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public TermsStatsFacet(string name, ICriteria criteria, string key, string value)
        {
            Argument.CheckNotEmpty("name", name);
            Argument.CheckNotEmpty("key", key);
            Argument.CheckNotEmpty("value", value);

            this.Name = name;
            this.Filter = criteria;
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the type of this facet as specified in the Elasticsearch DSL
        /// </summary>
        public string Type
        {
            get { return "terms_stats"; }
        }

        /// <summary>
        /// Gets the name of this facet as specified in the Elasticsearch DSL
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the criteria of this facet
        /// </summary>
        public ICriteria Filter { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the dimension of top terms should be returned out of the overall terms list
        /// </summary>
        public int? Size { get; private set; }
    }
}