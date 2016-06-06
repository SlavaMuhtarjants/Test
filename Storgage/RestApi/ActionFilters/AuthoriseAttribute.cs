namespace Weezlabs.Storgage.RestApi.ActionFilters
{
    using System.Web.Http;
    using System.Web.Http.Controllers;

    /// <summary>
    /// Injects additional logic to handling of unauthorised http responses
    /// </summary>
    public class AuthoriseAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Logs unauthorised http response codes
        /// </summary>
        /// <param name="actionContext">action context</param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
            actionContext.Log();
        }
    }
}