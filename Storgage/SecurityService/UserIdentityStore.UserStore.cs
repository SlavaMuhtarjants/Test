namespace Weezlabs.Storgage.SecurityService
{    
    using System;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface implementation of ASP Identity to manage by user account.
    /// </summary>
    public partial class UserIdentityStore : IUserStore<User, Guid>
    {
        /// <summary>
        /// Creates user account.
        /// </summary>
        /// <param name="user">User account to add.</param>
        /// <returns>Task.</returns>
        public Task CreateAsync(User user)
        {
            Contract.Requires(user != null);

            userRepository.Add(user);           

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Dletes user account.
        /// </summary>
        /// <param name="user">User account to delete.</param>
        /// <returns></returns>
        public Task DeleteAsync(User user)
        {
            Contract.Requires(user != null);

            userRepository.Delete(user);          

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Finds user by identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns></returns>
        public Task<User> FindByIdAsync(Guid userId)
        {
            var result = GetUsers()
                .SingleOrDefault(x => x.Id == userId);
            return Task.FromResult<User>(result);
        }

        /// <summary>
        /// Finds user by name.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <returns>User.</returns>
        public Task<User> FindByNameAsync(String userName)
        {
            var result = GetUsers()
                .SingleOrDefault(x => x.Email == userName);
            return Task.FromResult<User>(result);
        }

        /// <summary>
        /// Updates user account.
        /// </summary>
        /// <param name="user">User account to update.</param>
        /// <returns>User.</returns>
        public Task UpdateAsync(User user)
        {
            Contract.Requires(user != null);

            userRepository.Update(user);
            return Task.FromResult<Object>(null);
        }

        private IQueryable<User> GetUsers()
        {
            return
                userRepository.GetAll()
                    .Include(x => x.PhoneVerificationStatus)
                    .Include(x => x.EmailVerificationStatus);
        }
    }
}
