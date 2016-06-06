namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Object for add card response
    /// </summary>
    public class CardRequest
    {
        /// <summary>
        /// Card token
        /// </summary>
        [Required]
        public String CardToken { get; set; }

        /// <summary>
        /// True if should set card as default
        /// </summary>
        public Boolean SetDefault { get; set; }

        /// <summary>
        /// Create instance of card request object
        /// </summary>
        public CardRequest()
        {
            this.SetDefault = false;
        }
    }
}
