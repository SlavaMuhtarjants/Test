namespace Weezlabs.Storgage.DataLayer.Users
{
    using System;

    using Model;

    /// <summary>
    /// User repository specific interface.
    /// </summary>
    public interface IUserRepository : IRepository<User, Guid>
    {
        Boolean UserDel(String userXml);
    }
}
