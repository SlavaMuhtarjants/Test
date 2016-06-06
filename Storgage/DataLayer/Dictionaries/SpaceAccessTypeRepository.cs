namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Repository for access types.
    /// </summary>
    public class SpaceAccessTypeRepository : BaseReadonlyRepository<SpaceAccessType, Guid>, ISpaceAccessTypeReadonlyRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public SpaceAccessTypeRepository(DbContext context)
            :base(context)
        {
        }
    }
}
