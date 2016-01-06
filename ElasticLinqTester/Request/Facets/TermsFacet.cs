// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq.Request.Facets
{
    /// <summary>
    /// Represents a terms facet.
    /// Terms Facets return count information for terms.
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => a.Something).Select(a => a.Count())</remarks>
    [DebuggerDisplay("TermsFacet {Fields} {Filter}")]
    public class TermsFacet : IOrderableFacet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermsFacet"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="size">The size.</param>
        /// <param name="fields">The fields.</param>
        public TermsFacet(string name, int? size, params string[] fields)
            : this(name, null, size, fields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermsFacet"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="size">The size.</param>
        /// <param name="fields">The fields.</param>
        public TermsFacet(string name, ICriteria criteria, int? size, params string[] fields)
        {
            Argument.CheckNotEmpty("name", name);
            Argument.CheckNotEmpty("fields", fields);

            this.Name = name;
            this.Filter = criteria;
            this.Size = size;
            this.Fields = new ReadOnlyCollection<string>(fields);
        }

        /// <summary>
        /// Gets the type of this facet as specified in the Elasticsearch DSL
        /// </summary>
        public string Type
        {
            get { return "terms"; }
        }

        /// <summary>
        /// Gets the name of this facet as specified in the Elasticsearch DSL
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public ReadOnlyCollection<string> Fields { get; private set; }

        /// <summary>
        /// Gets the criteria of this facet
        /// </summary>
        public ICriteria Filter { get; private set; }

        /// <summary>
        /// Gets the dimension of top terms should be returned out of the overall terms list
        /// </summary>
        public int? Size { get; private set; }
    }
}