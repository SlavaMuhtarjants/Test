namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Chat member repository.
    /// </summary>
    public class ChatMemberRepository : BaseRepository<ChatMember, Guid>, IChatMemberRepository
    {
        /// <summary>
        /// Creates chat member repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public ChatMemberRepository(DbContext context) : base(context)
        { }
    }
}
