namespace Weezlabs.Storgage.SecurityService
{    
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface implementation of ASP Identity for claims.
    /// </summary>
    public partial class UserIdentityStore : IUserClaimStore<User, Guid>
    {
        /// <summary>
        /// Adds new claim. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="claim">New claim.</param>
        /// <returns>Task.</returns>
        public Task AddClaimAsync(User user, Claim claim)
        {
            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Returns claims of user. No Implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>List with claims.</returns>
        public Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return Task.FromResult<IList<Claim>>(new List<Claim>());
        }

        /// <summary>
        /// Remove claim. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="claim">Claim to delete.</param>
        /// <returns>Task.</returns>
        public Task RemoveClaimAsync(User user, Claim claim)
        {
            return Task.FromResult<Object>(null);
        }
    }
}
