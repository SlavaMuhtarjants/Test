namespace Weezlabs.Storgage.DataLayer.Filters
{
    using System;

    using Weezlabs.Storgage.Model;

    /// <summary>
    /// Interface for reading user filters dictionary
    /// </summary>
    public interface IFilterDictionaryReadonlyRepository : IReadonlyRepository<FilterRootDictionary, Guid>
    {

    }
}