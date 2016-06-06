namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Repository for space type.
    /// </summary>
    public class SpaceTypeRepository : BaseReadonlyRepository<SpaceType, Guid>, ISpaceTypeReadonlyRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public SpaceTypeRepository(DbContext context)
            : base(context)
        {
        }
    }
}
