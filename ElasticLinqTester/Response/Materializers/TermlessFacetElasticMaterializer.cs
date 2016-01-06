// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Linq;
using ElasticSearch.Linq.Response.Model;
using ElasticSearch.Linq.Utility;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Response.Materializers
{
    /// <summary>
    /// Materializes a single termless facet from the response.
    /// </summary>
    public class TermlessFacetElasticMaterializer : IElasticMaterializer
    {
        static readonly string[] termlessFacetTypes = { "statistical", "filter" };

        readonly Func<AggregateRow, object> projector;
        readonly Type elementType;
        readonly object key;

        /// <summary>
        /// Create an instance of the <see cref="TermlessFacetElasticMaterializer"/> with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired object.</param>
        /// <param name="elementType">The type of object being materialized.</param>
        /// <param name="key">The constant value for any key references during materialization.</param>
        public TermlessFacetElasticMaterializer(Func<AggregateRow, object> projector, Type elementType, object key = null)
        {
            Argument.CheckNotNull("projector", projector);
            Argument.CheckNotNull("elementType", elementType);

            this.projector = projector;
            this.elementType = elementType;
            this.key = key;
        }

        /// <summary>
        /// Materialize a single object from the response using the <see cref="projector"/>
       ///  or return a default value based on the element type.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> to materialize Facets from.</param>
        /// <returns>Object materialized from the response using the projector or default if no corresponding Facets.</returns>
        public virtual object Materialize(ElasticResponse response)
        {
            Argument.CheckNotNull("response", response);

            return MaterializeSingle(response) ?? TypeHelper.CreateDefault(elementType);
        }

        /// <summary>
        /// Materialize a single object from the response using the <see cref="projector"/>
        /// or return null if there are no applicable Facets.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> to materialize Facets from.</param>
        /// <returns>Object materialized from the response using the projector or null if no corresponding Facets.</returns>
        public object MaterializeSingle(ElasticResponse response)
        {
            Argument.CheckNotNull("response", response);

            var facets = response.Facets;
            if (facets != null && facets.Count > 0)
            {
                var facetsWithoutTerms = facets
                    .Values()
                    .Where(x => termlessFacetTypes.Contains(x["_type"].ToString()))
                    .ToList();

                if (facetsWithoutTerms.Any())
                    return projector(new AggregateStatisticalRow(key, facets));
            }

            return null;
        }

        /// <summary>
        /// Type of element being materialized.
        /// </summary>
        internal Type ElementType { get { return elementType; } }
    }
}