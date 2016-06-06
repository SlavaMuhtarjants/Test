namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Chat repository.
    /// </summary>
    public class ChatRepository : BaseRepository<Chat, Guid>, IChatRepository
    {
        
        /// <summary>
        /// Creates chat repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public ChatRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns unread message count for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Unread messages countt.</returns>
        public int GetUnreadMessagesCount(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
