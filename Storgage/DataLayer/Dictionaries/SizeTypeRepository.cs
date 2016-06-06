namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Repository for size types.
    /// </summary>
    public class SizeTypeRepository : BaseReadonlyRepository<SizeType, Guid>, ISizeTypeReadonlyRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public SizeTypeRepository(DbContext context)
            : base(context)
        {
        }
    }
}
