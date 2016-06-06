namespace Weezlabs.Storgage.SecurityService
{    
    using System;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface implementation of ASP Identity to manage by security stamps.
    /// </summary>
    public partial class UserIdentityStore : IUserSecurityStampStore<User, Guid>
    {
        /// <summary>
        /// Returns security stamp. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>Security stamp.</returns>
        public Task<String> GetSecurityStampAsync(User user)
        {
            return Task.FromResult<String>(String.Empty);
        }

        /// <summary>
        /// Sets up security stamp.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="stamp">Security stamp.</param>
        /// <returns>Task.</returns>
        public Task SetSecurityStampAsync(User user, String stamp)
        {
            return Task.FromResult<Object>(null);
        }
    }
}
