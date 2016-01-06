// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Linq.Expressions;
using ElasticSearch.Linq.Request.Facets;

namespace ElasticSearch.Linq.Request.Expressions
{
    /// <summary>
    /// An expression tree node that represents an Elasticsearch facet.
    /// </summary>
    class FacetExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FacetExpression"/> class.
        /// </summary>
        /// <param name="facet"><see cref="IFacet" /> to represent with this expression.</param>
        public FacetExpression(IFacet facet)
        {
            this.Facet = facet;
        }

        /// <summary>
        /// <see cref="IFacet" /> Gets the facet that is represented by this expression.
        /// </summary>
        public IFacet Facet { get; private set; }

        /// <inheritdoc/>
        public override ExpressionType NodeType
        {
            get { return ElasticExpressionType.Facet; }
        }

        /// <inheritdoc/>
        public override Type Type
        {
            get { return typeof(bool); }
        }

        /// <inheritdoc/>
        public override bool CanReduce
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Facet.ToString();
        }
    }
}