namespace Weezlabs.Storgage.RestApi.Helpers.HttpActionResultStatusCode
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Not acceptable action result
    /// </summary>
    public class NotAcceptableResult : IHttpActionResult
    {
        private readonly HttpRequestMessage request;
        private readonly String message;

        /// <summary>
        /// Creates not acceptable action result.
        /// </summary>
        /// <param name="request">Http request.</param>
        /// <param name="message">Message.</param>
        public NotAcceptableResult(HttpRequestMessage request, String message)
        {
            Contract.Requires(request != null);
            this.request = request;
            this.message = message;
        }

        /// <summary>
        /// Overrides execute.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Http response message.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = request.CreateErrorResponse(HttpStatusCode.NotAcceptable, message);

            return Task.FromResult(response);
        }
    }
}