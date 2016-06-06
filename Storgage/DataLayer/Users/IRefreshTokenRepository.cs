namespace Weezlabs.Storgage.DataLayer.Users
{
    using System;

    using Model;

    /// <summary>
    /// Interface for refresh token repository.
    /// </summary>
    public interface IRefreshTokenRepository: IRepository<RefreshToken, String>
    {
    }
}
