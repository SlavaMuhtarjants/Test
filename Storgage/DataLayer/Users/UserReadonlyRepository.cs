namespace Weezlabs.Storgage.DataLayer.Users
{
    using System;
    using System.Data.Entity;
    
    using Model;

    public class UserReadonlyRepository : BaseReadonlyRepository<User, Guid>, IUserReadonlyRepository
    {
        /// <summary>
        /// Creates instance of user readonlly repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        public UserReadonlyRepository(DbContext context)
            : base(context)
        {
        }
    }
}
