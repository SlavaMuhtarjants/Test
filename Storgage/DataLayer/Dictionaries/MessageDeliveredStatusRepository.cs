namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Repository for space type.
    /// </summary>
    public class MessageDeliveredStatusRepository : BaseReadonlyRepository<MessageDeliveredStatus, Guid>, IMessageDeliveredStatusReadonlyRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public MessageDeliveredStatusRepository(DbContext context)
            : base(context)
        {
        }
    }
}
