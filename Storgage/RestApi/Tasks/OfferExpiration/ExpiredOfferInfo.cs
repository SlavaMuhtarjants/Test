namespace Weezlabs.Storgage.RestApi.Tasks.OfferExpiration
{
    using System;

    /// <summary>
    /// Expited offer info.
    /// </summary>
    public class ExpiredOfferInfo
    {
        /// <summary>
        /// Offer identifier.
        /// </summary>
        public Guid OfferID { get; set; }

        /// <summary>
        /// User identifier.
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Chat identifier.
        /// </summary>
        public Guid ChatID { get; set; }
    }
}