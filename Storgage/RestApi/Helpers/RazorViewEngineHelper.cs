namespace Weezlabs.Storgage.RestApi.Helpers
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Razor view engine helper.
    /// </summary>
    public class RazorViewEngineHelper
    {
        /// <summary>
        /// Render view to string
        /// </summary>
        /// <param name="controllerName">Name of controller.</param>
        /// <param name="viewName">Full name of view.</param>
        /// <param name="viewData">Data model for view.</param>
        /// <returns>Rendered view as string</returns>
        public static string RenderViewToString(String controllerName, String viewName, Object viewData)
        {
            var context = HttpContext.Current;
            var contextBase = new HttpContextWrapper(context);

            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);

            var controllerContext = new ControllerContext(contextBase,
                routeData,
                new EmptyController());

            var razorViewEngine = new RazorViewEngine();

            ViewEngineResult razorViewResult = razorViewEngine.FindView(controllerContext,
                viewName,
                "",
                false);
            var writer = new StringWriter();

            var viewContext = new ViewContext(controllerContext,
                razorViewResult.View,
                new ViewDataDictionary(viewData),
                new TempDataDictionary(),
                writer);

            razorViewResult.View.Render(viewContext, writer);
            return writer.ToString();
        }

        /// <summary>
        /// Empty controller.
        /// </summary>
        private class EmptyController : ControllerBase
        {
            protected override void ExecuteCore()
            {
            }
        }
    }
}