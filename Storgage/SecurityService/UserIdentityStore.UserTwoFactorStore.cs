namespace Weezlabs.Storgage.SecurityService
{    
    using System;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface implementation of ASP Identity to manage by two factor signin.
    /// </summary>
    public partial class UserIdentityStore : IUserTwoFactorStore<User, Guid>
    {
        /// <summary>
        /// Checks that two factor signin is allowed.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>If true then two factor signin is allowed.</returns>
        public Task<Boolean> GetTwoFactorEnabledAsync(User user)
        {
            return Task.FromResult<Boolean>(false);
        }

        /// <summary>
        /// Sets up two factor signin availability.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="enabled">If true then two factor signin is allowed.</param>
        /// <returns>Task.</returns>
        public Task SetTwoFactorEnabledAsync(User user, Boolean enabled)
        {
            return Task.FromResult<Object>(null);
        }
    }
}
