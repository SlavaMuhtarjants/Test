namespace Weezlabs.Storgage.RestApi.ActionFilters
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Filters;
    using System.Web.Http.Tracing;

    using Helpers;

    /// <summary>
    /// Action filter to handle for Global application errors.
    /// </summary>
    public class GlobalExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
            ITraceWriter trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            trace.Error(context.Request,
                "Controller : " + context.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName +
                Environment.NewLine + "Action : " + context.ActionContext.ActionDescriptor.ActionName, context.Exception);

            Type exceptionType = context.Exception.GetType();

            if (exceptionType == typeof(ValidationException))
            {
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(context.Exception.Message),
                    ReasonPhrase = "ValidationException"
                };
                throw new HttpResponseException(resp);
            }

            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.Unauthorized));
            }

            throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.InternalServerError));
        }
    }
}