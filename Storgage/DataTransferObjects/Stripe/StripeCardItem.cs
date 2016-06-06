using Stripe;

namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System;
    using System.Diagnostics.Contracts;
    
    /// <summary>
    /// Model for stripe card
    /// </summary>
    public class StripeCardItem
    {
        /// <summary>
        /// Stripe card identifier
        /// </summary>
        public String CardId { get; set; }

        /// <summary>
        /// Card brand name
        /// </summary>
        public String Brand { get; set; }

        /// <summary>
        /// Last 4 symbols of card number
        /// </summary>
        public String Number { get; set; }

        /// <summary>
        /// Flag: true if card is default
        /// </summary>
        public Boolean Default { get; set; }

        /// <summary>
        /// Create instance of stripe card item
        /// </summary>
        /// <param name="card"></param>
        public StripeCardItem(StripeCard card)
        {
            Contract.Requires(card != null);

            this.CardId = card.Id;
            this.Brand = card.Brand;
            this.Number = card.Last4;
        }
    }
}
