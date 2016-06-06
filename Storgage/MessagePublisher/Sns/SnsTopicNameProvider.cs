namespace Weezlabs.Storgage.MessagePublisher.Sns
{
    using System;
    using System.Diagnostics.Contracts;

    using UtilService;

    /// <summary>
    /// Topic name provider.
    /// </summary>
    public static class SnsTopicNameProvider
    {
        /// <summary>
        /// Returns name for topic.
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="deviceToken">Device token.</param>
        /// <returns></returns>
        public static String GetName(IAppSettings appSettings, String deviceToken)
        {
            Contract.Requires(appSettings != null);

            var name = String.Format("{0}-{1}",
                appSettings.GetSetting<String>("SnsTopicNamePrefix"),
                deviceToken);
            return name;
        }
    }
}
