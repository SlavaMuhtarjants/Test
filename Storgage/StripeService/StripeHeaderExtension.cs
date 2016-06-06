namespace Weezlabs.Storgage.StripeService
{
    using System;

    using UtilService;
    
    using RestSharp;

    /// <summary>
    /// Extension for Rest request to Stripe
    /// </summary>
    public static class StripeHeaderExtension
    {
        private static IAppSettings AppSettings
        {
            get { return IoC.ContainerWrapper.Container.Resolve<IAppSettings>(); }
        }

        private static String StripeApiKey { get { return AppSettings.GetSetting<String>("StripeApiKey"); } }

        /// <summary>
        /// Add autorization header to request to stripe
        /// </summary>
        /// <param name="request"></param>
        public static void AddAutorizationHeader(this RestRequest request)
        {
            request.AddHeader("Authorization", "Bearer " + StripeApiKey);
        }
    }
}
