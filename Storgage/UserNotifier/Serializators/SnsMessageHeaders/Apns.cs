namespace Weezlabs.Storgage.UserNotifier.Serializators.SnsMessageHeaders
{    
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Apns header.
    /// </summary>
    internal sealed class Apns
    {
        /// <summary>
        /// Aps header.
        /// </summary>
        [JsonProperty("aps")]
        public Aps Aps { get; set; }

        /// <summary>
        /// Event type.
        /// </summary>
        [JsonProperty("event")]
        public String Event { get; set; }

        /// <summary>
        /// Object identifier.
        /// </summary>
        [JsonProperty("objectId")]
        public String ObjectId { get; set; }
    }
}
