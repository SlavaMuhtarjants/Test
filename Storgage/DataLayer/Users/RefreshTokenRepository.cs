namespace Weezlabs.Storgage.DataLayer.Users
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Refresh token repository
    /// </summary>
    public class RefreshTokenRepository : BaseRepository<RefreshToken, String>, IRefreshTokenRepository
    {
        /// <summary>
        /// Creates repository instance.
        /// </summary>
        /// <param name="context">Db context.</param>
        public RefreshTokenRepository(DbContext context) : base(context)
        { }
    }
}
