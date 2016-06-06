namespace Weezlabs.Storgage.SecurityService
{
    using System;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Preferences for resolving IIdentityMessageService implementations
    /// </summary>
    public static class IdentityMessageServiceResolver
    {
        private const String smsServiceInstanceName = "smsService";
        private const String emailServiceInstanceName = "emailService";

        /// <summary>
        /// Get name of sms service instance
        /// </summary>
        public static String GetSmsServiceInstanceName()
        {
            return smsServiceInstanceName;
        }

        /// <summary>
        /// Get name of email service instance
        /// </summary>
        public static String GetEmailServiceInstanceName()
        {
            return emailServiceInstanceName;
        }

        /// <summary>
        /// Get sms service instance
        /// </summary>
        /// <returns>IIdentityMessageService</returns>
        public static IIdentityMessageService GetSmsService()
        {
            return IoC.ContainerWrapper.Container.Resolve<IIdentityMessageService>(smsServiceInstanceName);
        }

        /// <summary>
        /// Get email service instance
        /// </summary>
        /// <returns>IIdentityMessageService</returns>
        public static IIdentityMessageService GetEmailService()
        {
            return IoC.ContainerWrapper.Container.Resolve<IIdentityMessageService>(emailServiceInstanceName);
        }
    }
}
