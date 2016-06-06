namespace Weezlabs.Storgage.SecurityService
{   
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface implmentation of ASP Identtity for user email.
    /// </summary>
    public partial class UserIdentityStore : IUserEmailStore<User, Guid>
    {
        /// <summary>
        /// Finds user by email.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <returns>User.</returns>
        public Task<User> FindByEmailAsync(String email)
        {
            return FindByNameAsync(email);
        }

        /// <summary>
        /// Returns email for user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>Email.</returns>
        public Task<String> GetEmailAsync(User user)
        {
            Contract.Requires(user != null);

            return Task.FromResult<String>(user.Email);
        }

        /// <summary>
        /// Returns status of email verification.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>True if email was verified..</returns>
        public Task<Boolean> GetEmailConfirmedAsync(User user)
        {
            Contract.Requires(user != null);

            var result = user.EmailVerificationStatus.ToEnum() == Model.Enums.EmailVerificationStatus.Verified;

            return Task.FromResult<Boolean>(result);
        }
        
        /// <summary>
        /// Sets email for user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="email">New email.</param>
        /// <returns>Task.</returns>
        public Task SetEmailAsync(User user, String email)
        {
            Contract.Requires(user != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(email));
            
            user.Email = email;

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Confirms user email.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="confirmed">True if email was verified with success.</param>
        /// <returns>Task.</returns>
        public Task SetEmailConfirmedAsync(User user, Boolean confirmed)
        {
            Contract.Requires(user != null);

            user.EmailVerificationStatusID =
                dictionaryProvider.EmailVerificationStatuses.Single(
                    x => x.ToEnum() == Model.Enums.EmailVerificationStatus.Verified).Id;

            return Task.FromResult<Object>(null);
        }
    }
}
