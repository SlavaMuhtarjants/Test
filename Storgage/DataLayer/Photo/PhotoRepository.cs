namespace Weezlabs.Storgage.DataLayer.Photo
{
    using System;
    using System.Data.Entity;

    using Model;

    public class PhotoRepository : BaseRepository<PhotoLibrary, Guid>, IPhotoRepository
    {
        /// <summary>
        /// Creates photo repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public PhotoRepository(DbContext context) : base(context)
        {
        }
    }
}
