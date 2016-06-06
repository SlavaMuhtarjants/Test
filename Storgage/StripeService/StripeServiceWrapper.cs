namespace Weezlabs.Storgage.StripeService
{
    using System;

    using UtilService;

    using Stripe;

    /// <summary>
    /// Stripe service wrapper
    /// </summary>
    public class StripeServiceWrapper
    {
        private IAppSettings appSettings
        {
            get { return IoC.ContainerWrapper.Container.Resolve<IAppSettings>(); }
        }

        private String StripeApiKey { get { return appSettings.GetSetting<String>("StripeApiKey"); } }

        private StripeAccountService stripeAccountService;
        private StripeCustomerService stripeCustomerService;
        private StripeCardService stripeCardService;
        private BankAccountService stripeBankAccountService;


        /// <summary>
        /// Returns stripe account service.
        /// </summary>
        public StripeAccountService StripeAccountService
        {
            get
            {
                if (stripeAccountService != null)
                {
                    return stripeAccountService;
                }
                stripeAccountService = new StripeAccountService(StripeApiKey);
                return stripeAccountService;
            }
        }

        /// <summary>
        /// Returns stripe customer service.
        /// </summary>
        public StripeCustomerService StripeCustomerService
        {
            get
            {
                if (stripeCustomerService != null)
                {
                    return stripeCustomerService;
                }
                stripeCustomerService = new StripeCustomerService(StripeApiKey);
                return stripeCustomerService;
            }
        }

        /// <summary>
        /// Returns stripe card service.
        /// </summary>
        public StripeCardService StripeCardService
        {
            get
            {
                if (stripeCardService != null)
                {
                    return stripeCardService;
                }
                stripeCardService = new StripeCardService(StripeApiKey);
                return stripeCardService;
            }
        }

        /// <summary>
        /// Returns stripe bank account service.
        /// </summary>
        public BankAccountService StripeBankAccountService
        {
            get
            {
                if (stripeBankAccountService != null)
                {
                    return stripeBankAccountService;
                }
                stripeBankAccountService = new BankAccountService(StripeApiKey);
                return stripeBankAccountService;
            }
        }
    }
}
