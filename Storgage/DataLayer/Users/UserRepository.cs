namespace Weezlabs.Storgage.DataLayer.Users
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// User repository.
    /// </summary>
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(DbContext context)
            : base(context)
        {
        }

        public Boolean UserDel (String userXml)
        {
            var se = (storgageEntities)Context;
            se.spUserDel(userXml);
            return true;
        }
    }
}
