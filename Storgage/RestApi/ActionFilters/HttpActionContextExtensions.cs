namespace Weezlabs.Storgage.RestApi.ActionFilters
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Tracing;

    using Helpers;

    /// <summary>
    /// Extends ActionFilterAttribute class by logging facility
    /// </summary>
    public static class HttpActionContextExtensions
    {
        /// <summary>
        /// Logs controller, action, parameters, statuc code information
        /// </summary>
        /// <param name="actionContext">action context</param>
        internal static void Log(this HttpActionContext actionContext)
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger("business"));
            ITraceWriter trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            trace.Warn(actionContext.Request, FormatDetailedInfo(actionContext, actionContext.Response.StatusCode),
                "JSON", actionContext.ActionArguments);
        }

        private static String FormatDetailedInfo(HttpActionContext context, HttpStatusCode status)
        {
            String controllerName = context.ControllerContext.ControllerDescriptor.ControllerType.FullName;
            String actionName = context.ActionDescriptor.ActionName;
            String breach = null;

            if (context.Response.Content is ObjectContent)
            {
                HttpError httpError = ((ObjectContent)context.Response.Content).Value as HttpError;

                if (httpError != null)
                {
                    breach = httpError.Message;
                }
            }

            return String.Format("Controller : {1}{0}Action : {2}{0}Status : {3}{0}Breach : {4}",
                Environment.NewLine, controllerName, actionName, status, breach);
        }
    }
}