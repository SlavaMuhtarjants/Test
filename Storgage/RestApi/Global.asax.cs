namespace Weezlabs.Storgage.RestApi
{
    using System;
    using System.Data.Entity;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.ModelBinding;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using Caching;
    using DataLayer.Dictionaries;
    using DependencyResolution;
    using IoC;
    using Model;
    using Model.Contracts;
    using SecurityService;
    using Tasks;

    using Castle.Facilities.Logging;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;  
    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
 

    /// <summary>
    /// Entry point to Rest API.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {        
       

        /// <summary>
        /// Start up event handler.
        /// </summary>
        protected void Application_Start()
        {
            InitDependencies(ContainerWrapper.Container);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            JsonConvert.DefaultSettings = (() =>
            {
                var settings = new JsonSerializerSettings();
                settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                settings.DateFormatString = "yyyy-MM-ddTHH:mm:sszzz";
                settings.Converters.Add(new StringEnumConverter());
                return settings;
            });
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = JsonConvert.DefaultSettings();
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorCompositionRoot(ContainerWrapper.Container));

            TaskScheduler.Start();
        }


        /// <summary>
        /// End up event handler.
        /// </summary>
        protected void Application_End()
        {
            TaskScheduler.Stop();
        }

        private static void InitDependencies(WindsorContainer container)
        {
            container.Install(FromAssembly.This());
            var controllerFactory = new WindsorControllerFactory(ContainerWrapper.Container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);            
            container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.NLog).WithAppConfig());
            container.AddFacility<TypedFactoryFacility>();
            container.Register(
                Component.For<ICacheProvider>().ImplementedBy<RuntimeCacheProvider>().LifestyleSingleton(),
                Component.For<ICollectionCacheProvider>().ImplementedBy<CollectionCacheProvider>().LifestyleSingleton(),
                Component.For<DbContext>().ImplementedBy<storgageEntities>().LifeStyle.HybridPerWebRequestPerThread());
            RegisterCustomUserManager(container);
            RegisterWebApiComponents(container);
            RegisterDictionaryComponents(container);
            container.Register(Component.For<IWindsorContainer>().Instance(container));
            container.RegisterComponentRegistrators();
            container.CallComponentRegistrators();
            container.RegisterAllComponentsFromExecutingDirectory();
        }

        /// <summary>
        /// Register CustomUserManager 
        /// With fixing problem with register component contained some properties with same interface
        /// </summary>
        /// <param name="container">IWindsorContainer</param>
        private static void RegisterCustomUserManager(IWindsorContainer container)
        {
            container.Register(Component.For<IIdentityMessageService>()
                .ImplementedBy<EmailService>()
                .LifestyleSingleton()
                .Named(IdentityMessageServiceResolver.GetEmailServiceInstanceName()).IsDefault(),

                Component.For<IIdentityMessageService>()
                    .ImplementedBy<SmsService>()
                    .LifestyleSingleton()
                    .Named(IdentityMessageServiceResolver.GetSmsServiceInstanceName()).IsDefault(),

                Component.For<UserManager<User, Guid>>()
                    .ImplementedBy<CustomUserManager>()
                    .LifeStyle.HybridPerWebRequestPerThread()
                    .DependsOn(Dependency.OnComponent(typeof(EmailService).Name,
                        IdentityMessageServiceResolver.GetEmailServiceInstanceName()))
                    .DependsOn(Dependency.OnComponent(typeof(SmsService).Name,
                        IdentityMessageServiceResolver.GetSmsServiceInstanceName())));
        }

        private static void RegisterWebApiComponents(WindsorContainer container)
        {
            container.Register(
                Component.For<IHttpControllerActivator>()
                    .ImplementedBy<WindsorCompositionRoot>()
                    .LifestyleSingleton(),

                Component.For<IHttpActionSelector>()
                    .ImplementedBy<ApiControllerActionSelector>()
                    .LifestyleTransient(),

                Component.For<IActionValueBinder>()
                    .ImplementedBy<DefaultActionValueBinder>()
                    .LifestyleTransient(),

                Component.For<IHttpActionInvoker>()
                    .ImplementedBy<ApiControllerActionInvoker>()
                    .LifestyleTransient(),
                
                Component.For<HttpConfiguration>()
                    .Instance(GlobalConfiguration.Configuration));
        }

        private static void RegisterDictionaryComponents(WindsorContainer container)
        {
            container.Register
            (
                Component.For<ISizeTypeReadonlyRepository>().ImplementedBy<SizeTypeRepository>().LifestyleTransient(),
                Component.For<ISpaceAccessTypeReadonlyRepository>().ImplementedBy<SpaceAccessTypeRepository>().LifestyleTransient(),
                Component.For<ISpaceTypeReadonlyRepository>().ImplementedBy<SpaceTypeRepository>().LifestyleTransient(),
                Component.For<IPhoneVerificationStatusReadonlyRepository>().ImplementedBy<PhoneVerificationStatusRepository>().LifestyleTransient(),
                Component.For<IEmailVerificationStatusReadonlyRepository>().ImplementedBy<EmailVerificationStatusRepository>().LifestyleTransient(),
                Component.For<IMessageDeliveredStatusReadonlyRepository>().ImplementedBy<MessageDeliveredStatusRepository>().LifestyleTransient(),
                Component.For<IMessageOfferStatusReadonlyRepository>().ImplementedBy<MessageeOfferStatusReadonlyRepository>().LifestyleTransient(),
                Component.For<IRentPeriodTypeReadonlyRepository>().ImplementedBy<RentPeriodTypeReadonlyRepository>().LifestyleTransient(),
                Component.For<IAbuseTypeDictionaryReadonlyRepository>().ImplementedBy<AbuseTypeDictionaryRepository>().LifestyleTransient(),                
                Component.For<IDictionaryProvider>().ImplementedBy<DictionaryProvider>().LifeStyle.HybridPerWebRequestPerThread()
            );
        }
    }
}
