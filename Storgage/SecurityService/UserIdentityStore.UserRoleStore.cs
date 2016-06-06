namespace Weezlabs.Storgage.SecurityService
{ 
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface implementation of ASP Identity to manage by user roles.
    /// </summary>
    public partial class UserIdentityStore : IUserRoleStore<User, Guid>
    {
        /// <summary>
        /// Adds new role to user. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="roleName">Role.</param>
        /// <returns>Task.</returns>
        public Task AddToRoleAsync(User user, String roleName)
        {
            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Returns roles for user. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>List of user roles.</returns>
        public Task<IList<String>> GetRolesAsync(User user)
        {
            return Task.FromResult<IList<String>>(new List<String>());
        }

        /// <summary>
        /// Checks that user has role. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="roleName">Role name.</param>
        /// <returns>If true then user has role.</returns>
        public Task<Boolean> IsInRoleAsync(User user, String roleName)
        {
            return Task.FromResult<Boolean>(true);
        }

        /// <summary>
        /// Removes role from user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="roleName">Role.</param>
        /// <returns>Task.</returns>
        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            return Task.FromResult<Object>(null);
        }
    }
}
