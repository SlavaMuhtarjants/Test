namespace Weezlabs.Storgage.DataLayer.Users
{
    using System;
    
    using Model;

    /// <summary>
    /// Repository for users devices.
    /// </summary>
    public interface IUserDeviceRepository : IRepository<UserDevice, Guid>
    {
    }
}
