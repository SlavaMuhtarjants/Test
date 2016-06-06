namespace Weezlabs.Storgage.DataLayer.Filters
{
    using System;

    using Model;

    /// <summary>
    /// Interface for reading user filters
    /// </summary>
    public interface IFilterReadonlyRepository : IReadonlyRepository<Filter, Guid>
    {

    }
}