namespace Weezlabs.Storgage.DataTransferObjects.Message
{  
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;

    using Model.Enums;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Offer information.
    /// </summary>
    public class Offer
    {
        /// <summary>
        /// Rent since.
        /// </summary>
        [Required]
        public DateTimeOffset RentSince { get; set; }

        /// <summary>
        /// Rent period type.
        /// </summary>
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public RentPeriodType RentPeriod { get; set; }
   
        /// <summary>
        /// Rate.
        /// </summary>
        [Required]
        [Range(0.0, 1000000000.0)]
        public Double Rate { get; set; } 
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Offer()
        { }      

        /// <summary>
        /// Creates DTO from model.
        /// </summary>
        /// <param name="offer">Model object.</param>
        public Offer (Model.MessageOffer offer)
        {
            Contract.Requires(offer != null);
            Rate = (Double)offer.Rate;
            RentPeriod = offer.RentPeriodType.ToEnum();
            RentSince = offer.RentSince;   
        }
    }
}
