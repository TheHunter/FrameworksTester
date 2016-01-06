using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using ElasticSearch.Linq.Mapping;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Request.Facets;
using ElasticSearch.Linq.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Request.Formatters
{
    /// <summary>
    /// Formats a SearchRequest into a JSON POST to be sent to Elasticsearch.
    /// </summary>
    public class SearchRequestFormatter
    {
        static readonly CultureInfo TransportCulture = CultureInfo.InvariantCulture;

        private readonly Lazy<string> body;
        private readonly IElasticConnection connection;
        private readonly IElasticMapping mapping;
        private readonly SearchRequest searchRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequestFormatter"/> class. 
        /// Create a new SearchRequestFormatter for the given connection, mapping and search request.
        /// </summary>
        /// <param name="connection">
        /// The ElasticConnectionImpl to prepare the SearchRequest for.
        /// </param>
        /// <param name="mapping">
        /// The IElasticMapping used to format the SearchRequest.
        /// </param>
        /// <param name="searchRequest">
        /// The SearchRequest to be formatted.
        /// </param>
        public SearchRequestFormatter(IElasticConnection connection, IElasticMapping mapping, SearchRequest searchRequest)
        {
            this.connection = connection;
            this.mapping = mapping;
            this.searchRequest = searchRequest;

            this.body = new Lazy<string>(() => this.MakeBody().ToString(connection.Options.Pretty ? Formatting.Indented : Formatting.None));
        }

        /// <summary>
        /// Gets the JSON formatted POST body for the request to be sent to Elasticsearch.
        /// </summary>
        public string Body
        {
            get { return this.body.Value; }
        }

        private static JToken Build(IFacet facet)
        {
            if (facet is StatisticalFacet)
                return Build((StatisticalFacet)facet);

            if (facet is TermsStatsFacet)
                return Build((TermsStatsFacet)facet);

            if (facet is TermsFacet)
                return Build((TermsFacet)facet);

            if (facet is FilterFacet)
                return new JObject();

            throw new InvalidOperationException(string.Format("Unknown implementation of IFacet {0} can not be formatted", facet.GetType().Name));
        }

        private static JToken Build(StatisticalFacet statisticalFacet)
        {
            return new JObject(
                BuildFieldProperty(statisticalFacet.Fields)
            );
        }

        private static JToken Build(TermsStatsFacet termStatsFacet)
        {
            return new JObject(
                new JProperty("key_field", termStatsFacet.Key),
                new JProperty("value_field", termStatsFacet.Value)
            );
        }

        private static JToken Build(TermsFacet termsFacet)
        {
            return new JObject(BuildFieldProperty(termsFacet.Fields));
        }

        private static JToken BuildFieldProperty(ReadOnlyCollection<string> fields)
        {
            return fields.Count == 1
                ? new JProperty("field", fields.First())
                : new JProperty("fields", new JArray(fields));
        }

        private static JArray Build(IEnumerable<SortOption> sortOptions)
        {
            return new JArray(sortOptions.Select(Build));
        }

        private static object Build(SortOption sortOption)
        {
            if (!sortOption.IgnoreUnmapped)
                return sortOption.Ascending
                    ? (object)sortOption.Name
                    : new JObject(new JProperty(sortOption.Name, "desc"));

            var properties = new List<JProperty> { new JProperty("ignore_unmapped", true) };
            if (!sortOption.Ascending)
                properties.Add(new JProperty("order", "desc"));

            return new JObject(new JProperty(sortOption.Name, new JObject(properties)));
        }

        private static JObject Build(Highlight highlight)
        {
            var fields = new JObject();

            foreach (var field in highlight.Fields)
                fields.Add(new JProperty(field, new JObject()));

            var queryStringCriteria = new JObject(new JProperty("fields", fields));

            if (!string.IsNullOrWhiteSpace(highlight.PostTag))
                queryStringCriteria.Add(new JProperty("post_tags", new JArray(highlight.PostTag)));
            if (!string.IsNullOrWhiteSpace(highlight.PreTag))
                queryStringCriteria.Add(new JProperty("pre_tags", new JArray(highlight.PreTag)));

            return queryStringCriteria;
        }

        private static JObject Build(QueryStringCriteria criteria)
        {
            var unformattedValue = criteria.Value; // We do not reformat query_string

            var queryStringCriteria = new JObject(new JProperty("query", unformattedValue));

            if (criteria.Fields.Any())
                queryStringCriteria.Add(new JProperty("fields", new JArray(criteria.Fields)));

            return new JObject(new JProperty(criteria.Name, queryStringCriteria));
        }

        private static JObject Build(RegexpCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Regexp))));
        }

        private static JObject Build(PrefixCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Prefix))));
        }

        private static JObject Build(SingleFieldCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty("field", criteria.Field))));
        }

        private static JObject Build(MatchAllCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name));
        }

        /// <summary>
        /// Create the Json HTTP request body for this request given the search query and connection.
        /// </summary>
        /// <returns>Json to be used to execute this query by Elasticsearch.</returns>
        private JObject MakeBody()
        {
            var root = new JObject();

            if (this.searchRequest.Fields.Any())
                root.Add("fields", new JArray(this.searchRequest.Fields));

            if (this.searchRequest.MinScore.HasValue)
                root.Add("min_score", this.searchRequest.MinScore.Value);

            root.Add("version", true);

            var queryRoot = root;

            // Filters cause a filtered query to be created
            if (this.searchRequest.Filter != null)
            {
                queryRoot = new JObject(new JProperty("filter", Build(this.searchRequest.Filter)));
                root.Add("query", new JObject(new JProperty("filtered", queryRoot)));
            }

            if (this.searchRequest.Query != null)
                queryRoot.Add("query", Build(this.searchRequest.Query));

            // adding the version on hits response.

            if (this.searchRequest.SortOptions.Any())
                root.Add("sort", Build(this.searchRequest.SortOptions));

            if (this.searchRequest.From > 0)
                root.Add("from", this.searchRequest.From);

            if (this.searchRequest.Highlight != null)
                root.Add("highlight", Build(this.searchRequest.Highlight));

            long? size = this.searchRequest.Size ?? this.connection.Options.SearchSizeDefault;
            if (size.HasValue && !this.searchRequest.Facets.Any())
                root.Add("size", size.Value);

            if (this.searchRequest.Facets.Any())
                root.Add("Facets", Build(this.searchRequest.Facets, size));

            if (this.connection.Timeout != TimeSpan.Zero)
                root.Add("timeout", Format(this.connection.Timeout));

            return root;
        }

        private JToken Build(IEnumerable<IFacet> facets, long? defaultSize)
        {
            return new JObject(facets.Select(facet => Build(facet, defaultSize)));
        }

        private JProperty Build(IFacet facet, long? defaultSize)
        {
            Argument.CheckNotNull("facet", facet);

            var specificBody = Build(facet);
            if (facet is IOrderableFacet)
            {
                var facetSize = ((IOrderableFacet)facet).Size ?? defaultSize;
                if (facetSize.HasValue)
                    specificBody["size"] = facetSize.Value.ToString(TransportCulture);
            }

            var namedBody = new JObject(new JProperty(facet.Type, specificBody));

            if (facet.Filter != null)
                namedBody["filter"] = Build(facet.Filter);

            return new JProperty(facet.Name, namedBody);
        }

        private JObject Build(ICriteria criteria)
        {
            if (criteria == null)
                return null;

            if (criteria is RangeCriteria)
                return Build((RangeCriteria)criteria);

            if (criteria is RegexpCriteria)
                return Build((RegexpCriteria)criteria);

            if (criteria is PrefixCriteria)
                return Build((PrefixCriteria)criteria);

            if (criteria is TermCriteria)
                return Build((TermCriteria)criteria);

            if (criteria is TermsCriteria)
                return Build((TermsCriteria)criteria);

            if (criteria is NotCriteria)
                return Build((NotCriteria)criteria);

            if (criteria is QueryStringCriteria)
                return Build((QueryStringCriteria)criteria);

            if (criteria is MatchAllCriteria)
                return Build((MatchAllCriteria)criteria);

            if (criteria is BoolCriteria)
                return Build((BoolCriteria)criteria);

            //// Base class formatters using name property

            if (criteria is SingleFieldCriteria)
                return Build((SingleFieldCriteria)criteria);

            if (criteria is CompoundCriteria)
                return Build((CompoundCriteria)criteria);

            throw new InvalidOperationException(string.Format("Unknown criteria type {0}", criteria.GetType()));
        }

        private JObject Build(RangeCriteria criteria)
        {
            // Range filters can be combined by field
            return new JObject(
                new JProperty(criteria.Name,
                    new JObject(new JProperty(criteria.Field,
                        new JObject(criteria.Specifications.Select(s =>
                            new JProperty(s.Name, this.mapping.FormatValue(criteria.Member, s.Value))).ToList())))));
        }

        private JObject Build(TermCriteria criteria)
        {
            return new JObject(
                new JProperty(criteria.Name, new JObject(
                    new JProperty(criteria.Field, this.mapping.FormatValue(criteria.Member, criteria.Value)))));
        }

        private JObject Build(TermsCriteria criteria)
        {
            var termsCriteria = new JObject(
                new JProperty(criteria.Field,
                    new JArray(criteria.Values.Select(x => this.mapping.FormatValue(criteria.Member, x)).Cast<object>().ToArray())));

            if (criteria.ExecutionMode.HasValue)
                termsCriteria.Add(new JProperty("execution", criteria.ExecutionMode.GetValueOrDefault().ToString()));

            return new JObject(new JProperty(criteria.Name, termsCriteria));
        }

        private JObject Build(NotCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, Build(criteria.Criteria)));
        }

        private JObject Build(CompoundCriteria criteria)
        {
            // A compound filter with one item can be collapsed
            return criteria.Criteria.Count == 1
                ? Build(criteria.Criteria.First())
                : new JObject(new JProperty(criteria.Name, new JArray(criteria.Criteria.Select(Build).ToList())));
        }

        private JObject Build(BoolCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(this.BuildProperties(criteria))));
        }

        private IEnumerable<JProperty> BuildProperties(BoolCriteria criteria)
        {
            if (criteria.Must.Any())
                yield return new JProperty("must", new JArray(criteria.Must.Select(Build)));

            if (criteria.MustNot.Any())
                yield return new JProperty("must_not", new JArray(criteria.MustNot.Select(Build)));

            if (criteria.Should.Any())
            {
                yield return new JProperty("should", new JArray(criteria.Should.Select(Build)));
                yield return new JProperty("minimum_should_match", 1);
            }
        }

        internal static string Format(TimeSpan timeSpan)
        {
            if (timeSpan.Milliseconds != 0)
                return timeSpan.TotalMilliseconds.ToString(TransportCulture);

            if (timeSpan.Seconds != 0)
                return timeSpan.TotalSeconds.ToString(TransportCulture) + "s";

            return timeSpan.TotalMinutes.ToString(TransportCulture) + "m";
        }
    }
}
