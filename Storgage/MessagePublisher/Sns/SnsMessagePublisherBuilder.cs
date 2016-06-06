namespace Weezlabs.Storgage.MessagePublisher.Sns
{
    using System;
    using System.Diagnostics.Contracts;

    using UtilService;

    using Amazon;
    using Amazon.SimpleNotificationService;

    /// <summary>
    /// Sns message publisher builder.
    /// </summary>
    public static class SnsMessagePublisherBuilder
    {
        /// <summary>
        /// Creates SNS publisher.
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        /// <returns>SNS publisher.</returns>
        public static IAmazonSimpleNotificationService CreateSnsPublisher(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            var regionName = appSettings.GetSetting<String>("AWSRegionEndpoint");
            var snsPublisher = new AmazonSimpleNotificationServiceClient(appSettings.GetSetting<String>("AWSAccessKey"),
                appSettings.GetSetting<String>("AWSSecretKey"),
                RegionEndpoint.GetBySystemName(regionName));

            return snsPublisher;
        }
    }
}
