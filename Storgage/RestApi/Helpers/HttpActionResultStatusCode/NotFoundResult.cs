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
    /// New NotFoundResult with text.
    /// </summary>
    public class NotFoundResultWithText: IHttpActionResult
    {
        /// <summary>
        /// Message.
        /// </summary>
        private readonly String message;

        /// <summary>
        /// Http request.
        /// </summary>
        private readonly HttpRequestMessage request;

        /// <summary>
        /// Creates instance of new result.
        /// </summary>
        /// <param name="message">Message about not found result.</param>
        /// <param name="request">Http request.</param>
        public NotFoundResultWithText(String message, HttpRequestMessage request)
        {
            Contract.Requires(request != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(message));
            

            this.message = message;
            this.request = request;
        }        

        /// <summary>
        /// Implements execution action result.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Http response.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = request.CreateErrorResponse(HttpStatusCode.NotFound, message);

            return Task.FromResult(response);
        }        
    }
}