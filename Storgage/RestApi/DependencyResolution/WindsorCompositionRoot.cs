namespace Weezlabs.Storgage.RestApi.DependencyResolution
{    
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;

    using Castle.Windsor;

    /// <summary>
    /// Override activator for http controller.
    /// </summary>
    public class WindsorCompositionRoot : IHttpControllerActivator
    {
        private readonly IWindsorContainer container;

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="container">IoC container.</param>
        public WindsorCompositionRoot(IWindsorContainer container)
        {
            Contract.Requires(container != null);
            this.container = container;
        }

        /// <summary>
        /// Override creating http controller.
        /// </summary>
        /// <param name="request">Http request.</param>
        /// <param name="controllerDescriptor">Http controller descriptor.</param>
        /// <param name="controllerType">Http controller type.</param>
        /// <returns>Created http controller.</returns>
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, 
            Type controllerType)
        {
            var controller = container.Resolve<IHttpController>(controllerType.FullName);

            request.RegisterForDispose(
                new Release(
                    () => container.Release(controller)));

            return controller;
        }

        /// <summary>
        /// Internal class for safety dispose.
        /// </summary>
        private class Release : IDisposable
        {
            private readonly Action release;

            /// <summary>
            /// Create instance.
            /// </summary>
            /// <param name="release">Release callback.</param>
            public Release(Action release)
            {
                Contract.Requires(release != null);
                this.release = release;
            }

            public void Dispose()
            {
                release();
            }
        }

    }
}