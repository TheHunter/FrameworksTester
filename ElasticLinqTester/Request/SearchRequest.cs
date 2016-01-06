using System.Collections.Generic;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Request.Facets;

namespace ElasticSearch.Linq.Request
{
    /// <summary>
    /// Represents a search request to send to Elasticsearch.
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequest"/> class. 
        /// Create a new SearchRequest.
        /// </summary>
        public SearchRequest()
        {
            this.Fields = new List<string>();
            this.SortOptions = new List<SortOption>();
            this.Facets = new List<IFacet>();
        }

        /// <summary>
        /// Gets or sets th index to start taking the Elasticsearch documents from.
        /// </summary>
        /// <remarks>Maps to the Skip statement of LINQ.</remarks>
        public long @From { get; set; }

        /// <summary>
        /// Gets or sets the number of documents to return from Elasticsearch.
        /// </summary>
        /// <remarks>Maps to the Take statement of LINQ.</remarks>       
        public long? Size { get; set; }

        /// <summary>
        /// Gets or sets the type of documents to return from Elasticsearch.
        /// </summary>
        /// <remarks>Derived from the T specified in Query&lt;T&gt;.</remarks>
        public string DocumentType { get; set; }

        /// <summary>
        /// Gets or sets the list of fields to return for each document instead of the
        /// </summary>
        public List<string> Fields { get; set; }

        /// <summary>
        /// Gets or sets the sort sequence for the documents. This affects From and Size.
        /// </summary>
        /// <remarks>Determined by the OrderBy/ThenBy LINQ statements.</remarks>
        public List<SortOption> SortOptions { get; set; }

        /// <summary>
        /// Gets or sets the filter criteria for the documents.
        /// </summary>
        /// <remarks>Determined by the Where LINQ statements.</remarks>
        public ICriteria Filter { get; set; }

        /// <summary>
        /// Gets or sets the query criteria for the documents.
        /// </summary>
        /// <remarks>Determined by the Query extension methods.</remarks>
        public ICriteria Query { get; set; }

        /// <summary>
        /// Gets or sets the facet aggregations and statistical information that could be included.
        /// </summary>
        /// <remarks>Determined by the GroupBy/Count/Sum/Average statements of LINQ.</remarks>
        public List<IFacet> Facets { get; set; }

        /// <summary>
        /// Gets or sets the type of search Elasticsearch should perform.
        /// </summary>
        /// <remarks>Is usually blank but can be set to Count when Facets are required instead of Hits.</remarks>
        public string SearchType { get; set; }

        /// <summary>
        /// Gets or sets the minimum score of results to be returned.
        /// </summary>
        public double? MinScore { get; set; }

        /// <summary>
        /// Gets or sets the Specify the highlighting to be applied to the results.
        /// </summary>
        public Highlight Highlight { get; set; }
    }
}
