﻿using ElasticSearch.Linq.Response.Model;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq.Response.Materializers
{
    abstract class ChainMaterializer : IElasticMaterializer
    {
        protected ChainMaterializer(IElasticMaterializer next)
        {
            Next = next;
        }

        public IElasticMaterializer Next
        {
            get; set;
        }


        /// <summary>
        /// Process response, then translate it to next materializer.
        /// </summary>
        /// <param name="response">ElasticResponse to obtain the existence of a result.</param>
        /// <returns>Return result of previous materializer, previously processed by self</returns>
        public virtual object Materialize(ElasticResponse response)
        {
            Argument.CheckNotNull("Next materializer must be setted.",Next);

            return Next.Materialize(response);
        }
    }
}