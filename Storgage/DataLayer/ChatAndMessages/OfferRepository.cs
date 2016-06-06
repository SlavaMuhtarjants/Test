namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Repository for offers.
    /// </summary>
    public class OfferRepository : BaseRepository<MessageOffer, Guid>, IOfferRepository
    {
        /// <summary>
        /// Creates readonly repositories for offers.
        /// </summary>
        /// <param name="context">Database context.</param>
        public OfferRepository(DbContext context)
            : base(context)
        { }
    }
}
