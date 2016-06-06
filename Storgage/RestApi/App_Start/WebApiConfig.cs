namespace Weezlabs.Storgage.RestApi
{
    using System.Web.Http;

    using ActionFilters;

    /// <summary>
    /// Contains settings for Web API.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register routings for Web API.
        /// </summary>
        /// <param name="config">Http config.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Registering action filter for logging
            #if DEBUG
                config.Filters.Add(new LoggingFilterAttribute());
            #endif
            config.Filters.Add(new GlobalExceptionAttribute());
            config.Filters.Add(new ValidateQueryParametersAttribute());
            config.Filters.Add(new ValidateModelAttribute());
            config.Filters.Add(new BusinessExceptionFilterAttribute());
            config.Filters.Add(new AuthoriseAttribute());
        }
    }
}
