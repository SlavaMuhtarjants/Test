namespace Weezlabs.Storgage.UserNotifier.Serializators
{
    using System;
    using System.Diagnostics.Contracts;

    using Model.Exceptions;
    using Notifications;

    using Newtonsoft.Json;
    using UtilService;
    using SnsMessageHeaders;

    /// <summary>
    /// APNS JSON message serializator.
    /// </summary>
    public class ApnsJsonMessageSerializator : DefaultJsonMessageSerializator
    {
        private IAppSettings appSettings;

        /// <summary>
        /// Creates Apns json serializator.
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        public ApnsJsonMessageSerializator(IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);
            this.appSettings = appSettings;              
        }

        /// <summary>
        /// Serializes message to APNS JSON format.
        /// </summary>        
        /// <param name="message">Message.</param>
        /// <returns>String with serialized message.</returns>
        protected override string DoSerializeMessage(UserNotification message)
        {
            try
            {
                var sound = appSettings.GetSetting<String>("ApnsSound");
                var isLive = appSettings.GetSetting<Boolean>("IsLive");
                var snsMessage = new SnsMessage(message, sound, isLive);
                var result = snsMessage.SerializeToJson();
                return result;
            }
            catch (JsonSerializationException ex)
            {
                throw new SerializationException(Resources.Messages.SerializerMessageException, ex);
            }
        }
    }
}
