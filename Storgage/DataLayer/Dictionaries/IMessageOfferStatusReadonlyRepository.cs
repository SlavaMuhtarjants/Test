namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;

    using Model;

    /// <summary>
    /// Interface for message offer status repository.
    /// </summary>
    public interface IMessageOfferStatusReadonlyRepository : IReadonlyRepository<MessageOfferStatus, Guid>
    {
    }
}
