namespace Weezlabs.Storgage.DataLayer.ChatAndMessages
{
    using System;    

    using Model;

    /// <summary>
    /// Repository for offers.
    /// </summary>
    public interface IOfferRepository : IRepository<MessageOffer, Guid>
    {
    }
}
