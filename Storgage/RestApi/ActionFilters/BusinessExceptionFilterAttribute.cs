namespace Weezlabs.Storgage.RestApi.ActionFilters
{
    using System;
    using System.Web.Http.Filters;

    /// <summary>
    /// Catches exceptions of the business logic
    /// </summary>
    public class BusinessExceptionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Intercepts 4XX HTTP status codes
        /// </summary>
        /// <param name="actionExecutedContext">action context</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null)
            {
                Int32 statusCode = (Int32)actionExecutedContext.Response.StatusCode;

                if (statusCode >= 400 && statusCode < 500)
                {
                    actionExecutedContext.ActionContext.Log();
                }
            }
        }
    }
}