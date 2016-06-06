namespace Weezlabs.Storgage.SecurityService
{    
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface implementation of ASP Identity for using external secret tokens to signin.
    /// </summary>
    public partial class UserIdentityStore : IUserLoginStore<User, Guid>
    {
        /// <summary>
        /// Adds external token.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="login">External token.</param>
        /// <returns>Task.</returns>
        public Task AddLoginAsync(User user, UserLoginInfo login)
        {
            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Finds user by external token.
        /// </summary>
        /// <param name="login">External token.</param>
        /// <returns>User.</returns>
        public Task<User> FindAsync(UserLoginInfo login)
        {
            var result = GetUsers()
                .SingleOrDefault(x => x.FacebookID == login.ProviderKey);
            return Task.FromResult<User>(result);
        }

        /// <summary>
        /// Returns tokens for user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>List with tokens.</returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            return Task.FromResult<IList<UserLoginInfo>>(new List<UserLoginInfo>());
        }

        /// <summary>
        /// Removes token from user accuont.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="login">External token.</param>
        /// <returns>Task.</returns>
        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            return Task.FromResult<Object>(null);
        }
    }
}
