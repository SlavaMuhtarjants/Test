namespace Weezlabs.Storgage.RestApi.DependencyResolution
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    /// <summary>
    /// Controller installer
    /// </summary>
    public class ControllersInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Install controller
        /// </summary>
        /// <param name="container">Container.</param>
        /// <param name="store">Store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Types.FromThisAssembly()
                .Pick().If(t => t.Name.EndsWith("Controller"))
                .Configure(configurer => configurer.Named(configurer.Implementation.Name))
                .LifestylePerWebRequest());
        }
    }
}