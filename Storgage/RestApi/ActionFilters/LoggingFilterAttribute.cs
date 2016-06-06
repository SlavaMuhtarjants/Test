namespace Weezlabs.Storgage.RestApi.ActionFilters
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Web.Http.Tracing;

    using Helpers;

    /// <summary>
    /// Action filter to handle for calling actions
    /// </summary>
    public class LoggingFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Occurs before the action method is invoked.
        /// Replaced default ITraceWriter to NLogger class
        /// </summary>
        /// <param name="filterContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
            ITraceWriter trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            trace.Info(filterContext.Request,
                "Controller : " + filterContext.ControllerContext.ControllerDescriptor.ControllerType.FullName +
                Environment.NewLine + "Action : " + filterContext.ActionDescriptor.ActionName, "JSON",
                filterContext.ActionArguments);
        }
    }
}