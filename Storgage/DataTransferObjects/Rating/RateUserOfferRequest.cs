namespace Weezlabs.Storgage.DataTransferObjects.Rating
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Rate user offer request.
    /// </summary>
    public class RateUserOfferRequest
    {
        /// <summary>
        /// User offer rank.
        /// </summary>
        [Required]
        [Range(1, 5)]
        public Int32 Rank { get; set; }

        /// <summary>
        /// Message.
        /// </summary>       
        [MaxLength(300)]
        public String Message { get; set; }
    }
}
