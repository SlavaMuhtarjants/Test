namespace Weezlabs.Storgage.RestApi.DependencyResolution
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Castle.MicroKernel;

    /// <summary>
    /// Windsor controller factory
    /// </summary>
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Create instanse of windsor controller factory
        /// </summary>
        /// <param name="kernel">IKernel.</param>
        public WindsorControllerFactory(IKernel kernel)
        {
            this._kernel = kernel;
        }

        /// <summary>
        /// Release controller
        /// </summary>
        /// <param name="controller">Controller.</param>
        public override void ReleaseController(IController controller)
        {
            _kernel.ReleaseComponent(controller);
        }

        /// <summary>
        /// Get controller instanse.
        /// </summary>
        /// <param name="requestContext">Request context.</param>
        /// <param name="controllerType">Controller type.</param>
        /// <returns>Controller</returns>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, String.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }
            return (IController)_kernel.Resolve(controllerType);
        }
    }
}