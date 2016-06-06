namespace Weezlabs.Storgage.SecurityService
{    
    using System;
    using System.Linq;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Implementation of ASP Identity interface.
    /// </summary>
    public partial class UserIdentityStore : IQueryableUserStore<User, Guid>
    {
        /// <summary>
        /// Returns IQueryable of users.
        /// </summary>
        public IQueryable<User> Users { get { return userRepository.GetAll(); }}
    }
}
