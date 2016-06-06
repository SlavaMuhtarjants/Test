namespace Weezlabs.Storgage.DataTransferObjects.Abuse
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Weezlabs.Storgage.Model.Enums;
    
    public class AbuseRequest
    {
        /// <summary>
        /// Message with abuse description 
        /// </summary>       
        [MaxLength(300)]
        public String Message { get; set; }

        /// <summary>
        /// Link o reporter 
        /// </summary>
        public Guid ReporterId { get; set; }

        /// <summary>
        /// List of abuse types
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IEnumerable<AbuseTypeDictionary> AbuseType { get; set; }

        /// <summary>
        /// If we need Contact us this attibute can be set
        /// </summary>
        [JsonConverter(typeof(ContactUsDictionary?))]
        public ContactUsDictionary? ContactUsType { get; set; }

        public AbuseRequest()
        {

        }
    }
}
