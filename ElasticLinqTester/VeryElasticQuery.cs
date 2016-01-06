using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ElasticSearch.Linq.Request;
using ElasticSearch.Linq.Request.Formatters;
using ElasticSearch.Linq.Request.Visitors;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq
{
    public class VeryElasticQuery<T, TQueryProvider> : IElasticQuery<T>
        where TQueryProvider : IElasticQueryProvider
    {
        readonly TQueryProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="VeryElasticQuery{T,TQueryProvider}"/> class. 
        /// Initializes a new instance of the <see>
        ///         <cref>ElasticQuery{T}</cref>
        ///     </see>
        ///     class.
        /// </summary>
        /// <param name="provider">
        /// The <see>
        ///         <cref>ElasticQueryProvider</cref>
        ///     </see>
        ///     used to execute the queries.
        /// </param>
        public VeryElasticQuery(TQueryProvider provider)
        {
            Argument.CheckNotNull("provider", provider);

            this.provider = provider;
            this.Expression = Expression.Constant(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VeryElasticQuery{T,TQueryProvider}"/> class. 
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="provider">
        /// The <see>
        ///         <cref>ElasticQueryProvider</cref>
        ///     </see>
        ///     used to execute the queries.
        /// </param>
        /// <param name="expression">
        /// The <see cref="Expression"/> that represents the LINQ query so far.
        /// </param>
        public VeryElasticQuery(TQueryProvider provider, Expression expression)
        {
            Argument.CheckNotNull("provider", provider);
            Argument.CheckNotNull("expression", expression);

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            this.provider = provider;
            this.Expression = expression;
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.provider.Execute(this.Expression)).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.provider.Execute(this.Expression)).GetEnumerator();
        }

        /// <inheritdoc/>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <inheritdoc/>
        public Expression Expression { get; private set; }

        /// <inheritdoc/>
        public IQueryProvider Provider
        {
            get { return this.provider; }
        }

        /// <inheritdoc/>
        public virtual QueryInfo ToQueryInfo()
        {
            var request = ElasticQueryTranslator.Translate(this.provider.Mapping, Expression);
            var formatter = new SearchRequestFormatter(this.provider.Connection, this.provider.Mapping, request.SearchRequest);

            return new QueryInfo(formatter.Body, this.provider.Connection.GetSearchUri(request.SearchRequest));
        }
    }
}
