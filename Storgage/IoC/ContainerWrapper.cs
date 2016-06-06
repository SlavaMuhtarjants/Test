namespace Weezlabs.Storgage.IoC
{
    using Castle.Windsor;

    /// <summary>
    /// IoC container wrapper.
    /// </summary>
    public static class ContainerWrapper
    {
        /// <summary>
        /// Returns IoC container.
        /// </summary>
        public static WindsorContainer Container { get { return container; } }
        private static readonly WindsorContainer container = new WindsorContainer();
    }
}
