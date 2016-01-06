// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Linq.Expressions;
using ElasticSearch.Linq.Request.Criteria;

namespace ElasticSearch.Linq.Request.Expressions
{
    /// <summary>
    /// An expression tree node that represents criteria.
    /// </summary>
    class CriteriaExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CriteriaExpression"/> class.
        /// </summary>
        /// <param name="criteria"><see cref="ICriteria" /> to represent with this expression.</param>
        public CriteriaExpression(ICriteria criteria)
        {
            this.Criteria = criteria;
        }

        /// <summary>
        /// <see cref="ICriteria" /> Gets the criteria from this expression.
        /// </summary>
        public ICriteria Criteria { get; private set; }

        /// <summary>
        /// Gets the node type of this <see cref="T:System.Linq.Expressions.Expression" />.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return ElasticExpressionType.Criteria; }
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.
        /// </summary>
        public override Type Type
        {
            get { return typeof(bool); }
        }

        /// <summary>
        /// Indicates that the node can be reduced to a simpler node. If this returns true, Reduce() can be called to produce the reduced form.
        /// </summary>
        public override bool CanReduce
        {
            get { return false; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Criteria.ToString();
        }
    }
}