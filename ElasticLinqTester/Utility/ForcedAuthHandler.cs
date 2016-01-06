using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearch.Linq.Utility
{
    /// <summary>
    /// Forces basic authorization values, since negotiation requires an extra round-trip, which significantly
    /// increases the time required to search the index.
    /// </summary>
    public class ForcedAuthHandler : DelegatingHandler
    {
        readonly string userName;
        readonly string password;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForcedAuthHandler"/> class. 
        /// Creates a new ForcedAuthHandler fora  given username and password.
        /// </summary>
        /// <param name="userName">
        /// User name to use for authorization.
        /// </param>
        /// <param name="password">
        /// Password to use for authorization.
        /// </param>
        /// <param name="innerHandler">
        /// HttpMessageHandler to further control HTTP message processing.
        /// </param>
        public ForcedAuthHandler(string userName, string password, HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
            this.userName = userName;
            this.password = password;
        }

        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(this.userName))
            {
                var credentials = string.Format("{0}:{1}", this.userName, this.password);
                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
