namespace Weezlabs.Storgage.IoC
{    
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    /// <summary>
    /// Extensions.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Initialization of instances that were created before.
        /// </summary>
        /// <param name="container">IoC container.</param>
        /// <param name="instance">Instance to initialize.</param>
        public static void BuildUp(this IWindsorContainer container, object instance)
        {
            instance.GetType().GetProperties()
                 .Where(property => property.CanWrite && property.PropertyType.IsPublic)
                 .Where(property => container.Kernel.HasComponent(property.PropertyType))
                 .ToList()
                 .ForEach(property => property.SetValue(instance, container.Resolve(property.PropertyType), null));
        }

        /// <summary>
        /// Registered all instances from assemblies from executing directory.
        /// </summary>
        /// <param name="container">IoC container.</param>
        /// <param name="selector">Types selector. Using this selector we can ignore types to loading.</param>
        public static void RegisterAllComponentsFromExecutingDirectory(this IWindsorContainer container,
            Func<Type, bool> selector = null)
        {
            container.Register(
                Classes.FromAssemblyInDirectory(new AssemblyFilter(AssemblyDirectory))
                    .Pick().If(x => selector == null && x.IsPublic || selector != null && selector(x))
                    .WithService.AllInterfaces()
                    .LifestyleTransient()
                );
        }

        /// <summary>
        /// Register component registrator.
        /// </summary>
        /// <param name="container">Ioc container.</param>
        public static void RegisterComponentRegistrators(this IWindsorContainer container)
        {
            container.RegisterAllComponentsFromExecutingDirectory(x => typeof(IRegisterComponents).IsAssignableFrom(x));
        }

        /// <summary>
        /// Register components.
        /// </summary>
        /// <param name="container">Ioc container.</param>
        public static void CallComponentRegistrators(this IWindsorContainer container)
        {           
            container.ResolveAll<IRegisterComponents>().ToList().ForEach(x => x.Register(container));
        }        

        /// <summary>
        /// Returns active directory.       
        /// </summary>
        static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }        
    }

}
