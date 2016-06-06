namespace Weezlabs.Storgage.SecurityService
{    
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface implementation of ASP Identity to manage by password.
    /// </summary>
    public partial class UserIdentityStore : IUserPasswordStore<User, Guid>
    {
        /// <summary>
        /// Returns password hash.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns><Password hash.</returns>
        public async Task<String> GetPasswordHashAsync(User user)
        {
            Contract.Requires(user != null);
            if (!String.IsNullOrWhiteSpace(user.Password))
            {
                return await Task.FromResult<String>(user.Password);
            }

            var userFromDb = userRepository.GetById(user.Id);
            if (userFromDb != null)
            {
                return await Task.FromResult<String>(userFromDb.Password);
            }

            return await Task.FromResult<String>(null);
        }

        /// <summary>
        /// Returns true if password is setup.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>If true then password is setup.</returns>
        public async Task<Boolean> HasPasswordAsync(User user)
        {
            Contract.Requires(user != null);

            if (!String.IsNullOrWhiteSpace(user.Password))
            {
                return await Task.FromResult<Boolean>(true);
            }

            var passwordHash = await GetPasswordHashAsync(user);            
            
            return await Task.FromResult<Boolean>(!String.IsNullOrWhiteSpace(passwordHash));
        }

        /// <summary>
        /// Sets up password hash.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="passwordHash">New password hash.</param>
        /// <returns>Task.</returns>
        public Task SetPasswordHashAsync(User user, String passwordHash)
        {
            Contract.Requires(user != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(passwordHash));

            user.Password = passwordHash;           

            return Task.FromResult<Object>(null);
        }
    }
}
