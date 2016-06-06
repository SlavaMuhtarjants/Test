namespace Weezlabs.Storgage.DataLayer.Spaces
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using Model;
    using System.Data.Entity.Core.Objects;

    /// <summary>
    /// repository for spaces.
    /// </summary>
    public class SpaceRepository : BaseRepository<Space, Guid>, ISpaceReadonlyRepository, ISpaceRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public SpaceRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns spaces.
        /// </summary>
        /// <returns>Spaces.</returns>
        protected override IQueryable<Space> DoGetAll()
        {
            return base.DoGetAll();
        }

        public Decimal GetForecastedRate(Guid sizeTypeId, String zipCode)
        {
            var se = (storgageEntities)Context;

            var q =
                from z in se.Zips
                from f in se.fnSpaceForecastRateTable(sizeTypeId, z.Id)
                where z.ZipCode == zipCode
                select f;

            var r = q.SingleOrDefault();

            if (r == null)
            {
                return 0;
            }
            return r.MedianDisc;

        }
    }
}
