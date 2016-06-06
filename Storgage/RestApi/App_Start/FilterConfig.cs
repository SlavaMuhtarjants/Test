namespace Weezlabs.Storgage.RestApi
{
    using System.Web.Mvc;

    /// <summary>
    /// Filter config.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Register settings for filters.
        /// </summary>
        /// <param name="filters">Filters collection.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
