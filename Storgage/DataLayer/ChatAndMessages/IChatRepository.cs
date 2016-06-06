namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;

    using Model;

    /// <summary>
    /// Interface repository for chat.
    /// </summary>
    public interface IChatRepository : IRepository<Chat, Guid>
    {
        /// <summary>
        /// Returns unread message count for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Unread messages countt.</returns>
        Int32 GetUnreadMessagesCount(Guid userId);
    }
}
