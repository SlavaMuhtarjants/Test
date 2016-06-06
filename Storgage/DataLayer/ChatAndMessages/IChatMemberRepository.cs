namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;
    
    using Model;

    /// <summary>
    /// Interface for chat member repository.
    /// </summary>
    public interface IChatMemberRepository : IRepository<ChatMember, Guid>
    {
    }
}
