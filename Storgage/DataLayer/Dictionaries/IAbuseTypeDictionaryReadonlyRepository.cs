namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;

    using Model;

    /// <summary>
    /// Inteface for AbuseTypeDictionary readonly repository.
    /// </summary>
    public interface IAbuseTypeDictionaryReadonlyRepository : IReadonlyRepository<AbuseTypeDictionary, Guid>
    {
    }
}