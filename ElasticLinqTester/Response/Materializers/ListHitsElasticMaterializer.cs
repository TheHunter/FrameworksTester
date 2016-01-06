// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ElasticSearch.Linq.Response.Model;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq.Response.Materializers
{
    /// <summary>
    /// Materializes multiple Hits into a list of CLR objects.
    /// </summary>
    class ListHitsElasticMaterializer : IElasticMaterializer
    {
        static readonly MethodInfo ManyMethodInfo = typeof(ListHitsElasticMaterializer).GetMethodInfo(f => f.Name == "Many" && f.IsStatic);

        readonly Func<Hit, object> projector;
        readonly Type elementType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListHitsElasticMaterializer"/> class. 
        /// Create an instance of the ListHitsElasticMaterializer with the given parameters.
        /// </summary>
        /// <param name="projector">
        /// A function to turn a hit into a desired CLR object.
        /// </param>
        /// <param name="elementType">
        /// The type of CLR object being materialized.
        /// </param>
        public ListHitsElasticMaterializer(Func<Hit, object> projector, Type elementType)
        {
            this.projector = projector;
            this.elementType = elementType;
        }

        /// <summary>
        /// Materialize the Hits from the response into desired CLR objects.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> containing the Hits to materialize.</param>
        /// <returns>List of <see cref="elementType"/> objects as constructed by the <see cref="projector"/>.</returns>
        public object Materialize(ElasticResponse response)
        {
            Argument.CheckNotNull("response", response);

            var hits = response.Hits;
            if (hits == null || hits.Hits == null || !hits.Hits.Any())
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            return ManyMethodInfo
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { hits.Hits, projector });
        }

        internal static List<T> Many<T>(IEnumerable<Hit> hits, Func<Hit, object> projector)
        {
            return hits.Select(projector).Cast<T>().ToList();
        }
    }
}