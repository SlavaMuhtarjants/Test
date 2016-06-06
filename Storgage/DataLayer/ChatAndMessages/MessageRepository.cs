
namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Message repository.
    /// </summary>
    public class MessageRepository : BaseRepository<Message, Guid>, IMessageRepository
    {
        /// <summary>
        /// Creates message repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public MessageRepository(DbContext context)
            : base(context)
        {
        }
    }
}
