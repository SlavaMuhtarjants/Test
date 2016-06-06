namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Message offer status repository.
    /// </summary>
    public class MessageeOfferStatusReadonlyRepository : BaseReadonlyRepository<MessageOfferStatus, Guid>, 
        IMessageOfferStatusReadonlyRepository
    {
        /// <summary>
        /// Creates repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public MessageeOfferStatusReadonlyRepository(DbContext context)
            : base(context)
        {
        }
    }
}
