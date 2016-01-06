// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using ElasticSearch.Linq.Response.Model;

namespace ElasticSearch.Linq.Response.Materializers
{
    /// <summary>
    /// Materializes a count operation by obtaining the total Hits from the response.
    /// </summary>
    class CountElasticMaterializer : IElasticMaterializer
    {
        /// <summary>
        /// Materialize the result count for a given response.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> to obtain the count value from.</param>
        /// <returns>The ewaulr count expressed as either an int or long depending on the size of the count.</returns>
        public object Materialize(ElasticResponse response)
        {
            if (response.Hits.Total < 0)
                throw new ArgumentOutOfRangeException("response", "Contains a negative number of Hits.");

            if (response.Hits.Total <= int.MaxValue)
                return (int)response.Hits.Total;

            return response.Hits.Total;
        }
    }
}