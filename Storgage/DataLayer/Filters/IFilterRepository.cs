namespace Weezlabs.Storgage.DataLayer.Filters
{
    using System;

    using Model;

    /// <summary>
    /// Interface for mofifying user filters
    /// </summary>
    public interface IFilterRepository : IRepository<Filter, Guid>
    {

    }
}