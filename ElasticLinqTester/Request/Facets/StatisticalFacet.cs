// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq.Request.Facets
{
    /// <summary>
    /// Represents a stastical facet.
    /// Statistical Facets return all statistical information such
    /// as counts, sums, mean etc. for a given number of fields
    /// within the documents specified by the filter criteria.
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => 1).Select(a => a.Count(b => b.SomeField))</remarks>
    [DebuggerDisplay("StatisticalFacet {Fields} {Filter}")]
    public class StatisticalFacet : IFacet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticalFacet"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fields">The fields.</param>
        public StatisticalFacet(string name, params string[] fields)
            : this(name, null, fields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticalFacet"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="fields">The fields.</param>
        public StatisticalFacet(string name, ICriteria criteria, params string[] fields)
        {
            Argument.CheckNotEmpty("name", name);
            Argument.CheckNotEmpty("fields", fields);

            this.Name = name;
            this.Filter = criteria;
            this.Fields = new ReadOnlyCollection<string>(fields);
        }

        /// <summary>
        /// Gets the type of this facet as specified in the Elasticsearch DSL
        /// </summary>
        public string Type
        {
            get { return "statistical"; }
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
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public ReadOnlyCollection<string> Fields { get; private set; }
    }
}