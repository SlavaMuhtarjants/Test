namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System;

    /// <summary>
    /// Stripe customer source.
    /// </summary>
    public class StripeCustomerSource
    {
        /// <summary>
        /// Stripe card full info (just for getting back when creating new card)
        /// </summary>
        public StripeCardItem Card { get; set; }

        /// <summary>
        /// Customer's source Id 
        /// </summary>
        public String CardId { get; set; }

        /// <summary>
        /// Customer's source tokenId
        /// </summary>
        public String CardToken { get; set; }
        
        /// <summary>
        /// Set default for payment method
        /// </summary>
        public Boolean SetDefault { get; set; }
    }
}
