namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;

    using Model;

    /// <summary>
    /// Repository interface for messages.
    /// </summary>
    public interface IMessageRepository : IRepository<Message, Guid>
    {
    }
}
