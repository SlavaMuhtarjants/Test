namespace Weezlabs.Storgage.UserNotifier.Serializators.SnsMessageHeaders
{
    using System;
    using System.Diagnostics.Contracts;

    using Newtonsoft.Json;

    using Notifications;

    /// <summary>
    /// Sns message.
    /// </summary>
    internal sealed class SnsMessage
    {
        [JsonProperty("default")]
        internal String Default { get; set; }

        [JsonProperty("APNS")]
        internal String ApnsLive { get; set; }

        [JsonProperty("APNS_SANDBOX")]
        internal String ApnsSandbox { get; set; }

        /// <summary>
        /// Constructor for Amazon sns message.
        /// </summary>
        /// <param name="notification">User notification.</param>
        /// <param name="sound">Sound.</param>
        /// <param name="isLive">If true then we send message to APNS otherwise we send message to APNS_SANDBOX.</param>
        internal SnsMessage(UserNotification notification, String sound, Boolean isLive = true)
        {
            Contract.Requires(notification != null);

            Default = notification.Message;
            var apnsSerialized = JsonConvert.SerializeObject(new Apns
            {
                Aps = new Aps { Alert = notification.Message, Sound = sound, Badge = notification.Badge },
                Event = notification.EventType.ToString(),
                ObjectId = notification.ObjectId.ToString()
            });

            if (isLive)
            {
                ApnsLive = apnsSerialized;
            }
            else
            {
                ApnsSandbox = apnsSerialized;
            }
        }

        /// <summary>
        /// Seld serialization.
        /// </summary>
        /// <returns>Serialized Amazon sns message.</returns>
        internal String SerializeToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
