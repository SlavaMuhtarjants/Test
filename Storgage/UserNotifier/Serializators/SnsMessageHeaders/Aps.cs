namespace Weezlabs.Storgage.UserNotifier.Serializators.SnsMessageHeaders
{    
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Aps header.
    /// </summary>
    internal sealed class Aps
    {
        /// <summary>
        /// Alert of message.
        /// </summary>
        [JsonProperty("alert")]
        internal String Alert { get; set; }

        /// <summary>
        /// Badge of message.
        /// </summary>
        [JsonProperty("badge")]
        internal Int32? Badge { get; set; }

        /// <summary>
        /// Sound of message.
        /// </summary>
        [JsonProperty("sound")]
        internal String Sound { get; set; }
    }
}
