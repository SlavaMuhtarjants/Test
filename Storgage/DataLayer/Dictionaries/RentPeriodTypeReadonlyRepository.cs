namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Readonly repository for rent period types.
    /// </summary>
    public class RentPeriodTypeReadonlyRepository : BaseReadonlyRepository<RentPeriodType, Guid>, IRentPeriodTypeReadonlyRepository
    {
        public RentPeriodTypeReadonlyRepository(DbContext context)
            : base(context)
        { }
    }
}
