namespace Weezlabs.Storgage.DataLayer.Photo
{
    using System;

    using Model;

    /// <summary>
    /// Repository interface for photos.
    /// </summary>
    public interface IPhotoRepository : IRepository<PhotoLibrary, Guid>
    {
    }
}
