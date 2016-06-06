namespace Weezlabs.Storgage.DataLayer.Spaces
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using Model;

    /// <summary>
    /// Readonly repository for Zip codes.
    /// </summary>
    public class ZipRepository : BaseRepository<Zip, Guid>, IZipRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public ZipRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns zips.
        /// </summary>
        /// <returns>Zips</returns>
        protected override IQueryable<Zip> DoGetAll()
        {
            return base.DoGetAll();
        }
    }
}
