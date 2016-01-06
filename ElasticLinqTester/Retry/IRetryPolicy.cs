using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearch.Linq.Retry
{
    /// <summary>
    /// A policy that can perform an operation one or more times.
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// Attempts an asynchronous operation one or more times.
        /// </summary>
        /// <typeparam name="TResult">The result type from the operation.</typeparam>
        /// <param name="operationFunc">The lambda which performs the operation once.</param>
        /// <param name="shouldRetryFunc">The lambda which inspects a result and/or exception and decides whether it should retry the result.</param>
        /// <param name="appendLogInfoFunc">The lambda which can supplement info logging for failed searches.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A task with the completed result.</returns>
        Task<TResult> ExecuteAsync<TResult>(
            Func<CancellationToken, Task<TResult>> operationFunc,
            Func<TResult, Exception, bool> shouldRetryFunc,
            Action<TResult, Dictionary<string, object>> appendLogInfoFunc = null,
            CancellationToken cancellationToken = new CancellationToken());
    }
}
