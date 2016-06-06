namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;
    using System.Data.Entity;

    using Model;
    

    /// <summary>
    /// Offer history repository.
    /// </summary>
    public class OfferHistoryRepository : BaseRepository<MessageOfferHistory, Guid>, IOfferHistoryRepository
    {
        /// <summary>
        /// Create instance of repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public OfferHistoryRepository(DbContext context) : base(context)
        { }
    }
}
