
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using ElasticSearch.Linq.Mapping;
using ElasticSearch.Linq.Request;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Request.Visitors;
using ElasticSearch.Linq.Response.Model;
using ElasticSearch.Linq.Retry;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// An elastic query provider for making IQueryable instances.
    /// </summary>
    public class VeryElasticQueryProvider
        : IElasticQueryProvider
    {
        readonly ElasticRequestProcessor requestProcessor;

        public VeryElasticQueryProvider(IElasticConnection connection, IElasticMapping mapping, IRetryPolicy retryPolicy)
        {
            Argument.CheckNotNull("connection", connection);
            Argument.CheckNotNull("mapping", mapping);
            Argument.CheckNotNull("retryPolicy", retryPolicy);

            this.Connection = connection;
            this.Mapping = mapping;
            this.RetryPolicy = retryPolicy;

            this.requestProcessor = new ElasticRequestProcessor(connection, mapping, retryPolicy);
        }
        
        internal IRetryPolicy RetryPolicy { get; private set; }

        public IElasticConnection Connection { get; private set; }
        
        public IElasticMapping Mapping { get; private set; }

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetSequenceElementType(expression.Type);
            var queryType = typeof(VeryElasticQuery<,>).MakeGenericType(elementType);
            try
            {
                return (IQueryable)Activator.CreateInstance(queryType, this, expression);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return null;  // Never called, as the above code re-throws
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (!typeof(IQueryable<TElement>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            return new VeryElasticQuery<TElement, VeryElasticQueryProvider>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return AsyncHelper.RunSync(() => this.ExecuteAsync(expression));
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)this.Execute(expression);
        }
        
        public async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
        {
            return (TResult)await this.ExecuteAsync(expression, cancellationToken);
        }

        public async Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = new CancellationToken())
        {
            var translation = ElasticQueryTranslator.Translate(this.Mapping, expression);

            try
            {
                ElasticResponse response;
                if (translation.SearchRequest.Filter == ConstantCriteria.False)
                {
                    response = new ElasticResponse();
                }
                else
                {
                    response = await this.requestProcessor.SearchAsync(translation.SearchRequest, cancellationToken);
                    if (response == null)
                        throw new InvalidOperationException("No HTTP response received.");
                }

                return translation.Materializer.Materialize(response);
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return null;  // Never called, as the above code re-throws
            }
        }
    }
}
