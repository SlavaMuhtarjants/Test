namespace Weezlabs.Storgage.DataLayer.Users
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Repository for user devices.
    /// </summary>
    public class UserDeviceRepository : BaseRepository<UserDevice, Guid>, IUserDeviceRepository
    {
        /// <summary>
        /// Creates repository instance.
        /// </summary>
        /// <param name="context">Db context.</param>
        public UserDeviceRepository(DbContext context) : base(context)
        { }
    }
}
