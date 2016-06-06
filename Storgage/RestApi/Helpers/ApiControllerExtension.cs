namespace Weezlabs.Storgage.RestApi.Helpers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Web.Http;

    using HttpActionResultStatusCode;

    /// <summary>
    /// Api controller extensions.
    /// </summary>
    public static class ApiControllerExtension
    {
        /// <summary>
        /// Not modified action result extension.
        /// </summary>
        /// <param name="controller">Api controller.</param>
        /// <param name="message">Message.</param>
        /// <returns>304. Not modified.</returns>
        public static IHttpActionResult NotModified(this ApiController controller, String message)
        {
            Contract.Requires(controller != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(message));

            var result = new NotModifiedResult(controller.Request, message);
            return result;
        }

        /// <summary>
        /// Forbidden action result extension.
        /// </summary>
        /// <param name="controller">Api controller.</param>
        /// <param name="message">Message.</param>
        /// <returns>403. Forbidden result.</returns>
        public static IHttpActionResult Forbidden(this ApiController controller, String message)
        {
            Contract.Requires(controller != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(message));

            var result = new ForbiddenResult(controller.Request, message);
            return result;
        }

        /// <summary>
        /// Extension for NotFound result.
        /// </summary>
        /// <param name="controller">Api controller.</param>
        /// <param name="message">Not found error.</param>
        /// <returns>404. Not found result.</returns>
        public static IHttpActionResult NotFound(this ApiController controller, String message)
        {
            Contract.Requires(controller != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(message));

            var result = new NotFoundResultWithText(message, controller.Request);
            return result;
        }

        /// <summary>
        /// Not acceptable action result extension
        /// </summary>
        /// <param name="controller">Api controller.</param>
        /// <param name="message">Message.</param>
        /// <returns>406. Not acceptable result.</returns>
        public static IHttpActionResult NotAcceptable(this ApiController controller, String message)
        {
            Contract.Requires(controller != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(message));

            var result = new NotAcceptableResult(controller.Request, message);
            return result;
        }

        /// <summary>
        /// Service unavailable action result extension
        /// </summary>
        /// <param name="controller">Api controller.</param>
        /// <param name="message">Message.</param>
        /// <returns>503. Service unavailable result.</returns>
        public static IHttpActionResult ServiceUnavailable(this ApiController controller, String message)
        {
            Contract.Requires(controller != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(message));

            var result = new ServiceUnavailableResult(controller.Request, message);
            return result;
        }
    }
}