using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Linq.Retry
{
    /// <summary>
    /// The exception that is thrown when an operation does not succeed within a specified number of attempts.
    /// </summary>
#if !PCL
    [Serializable]
#endif
    public class RetryFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryFailedException"/> specifying the number of attempts.
        /// </summary>
        /// <param name="maxAttempts">Number of attempts tried.</param>
        public RetryFailedException(int maxAttempts)
            : base(string.Format("The operation did not succeed after the maximum number of retries ({0}).", maxAttempts)) { }
    }
}
