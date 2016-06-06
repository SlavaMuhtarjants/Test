namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;    

    using Model;

    /// <summary>
    /// Interface repository for message offer history.
    /// </summary>
    public interface IOfferHistoryRepository : IRepository<MessageOfferHistory, Guid>
    {
    }
}
