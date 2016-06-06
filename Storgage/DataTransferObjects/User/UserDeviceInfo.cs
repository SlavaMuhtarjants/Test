namespace Weezlabs.Storgage.DataTransferObjects.User
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Model.Enums;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;   

    /// <summary>
    /// User device.
    /// </summary>
    public class UserDeviceInfo
    {
        /// <summary>
        /// User device token.
        /// </summary>
        [Required]
        [MaxLength(128)]
        public String DeviceToken { get; set; }

        /// <summary>
        /// Mobile endpoint type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [Required]
        public MobileEndpointType MobileEndpointType { get; set; }

        /// <summary>
        /// True if push notification is enabled.
        /// </summary>
        public Boolean IsPushNotificationEnabled { get; set; }
    }
}
