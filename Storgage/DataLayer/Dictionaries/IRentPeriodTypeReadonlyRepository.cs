namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    
    using Model;

    /// <summary>
    /// Readonly repository for rent period types.
    /// </summary>
    public interface IRentPeriodTypeReadonlyRepository : IReadonlyRepository<RentPeriodType, Guid>
    {
    }
}
