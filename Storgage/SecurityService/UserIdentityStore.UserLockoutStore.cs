namespace Weezlabs.Storgage.SecurityService
{    
    using System;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Inrterface implementation of ASP Identity for user lockout.
    /// </summary>
    public partial class UserIdentityStore : IUserLockoutStore<User, Guid>
    {
        /// <summary>
        /// Returns acceess failed counter. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns><Access failed counter./returns>
        public Task<Int32> GetAccessFailedCountAsync(User user)
        {            
            return Task.FromResult<Int32>(user.AccessFailedCount);
        }

        /// <summary>
        /// Returns status of user blocking. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>True if user is blocked.</returns>
        public Task<Boolean> GetLockoutEnabledAsync(User user)
        {
            return Task.FromResult<Boolean>(true);
        }

        /// <summary>
        /// Returns date time offset when user lock is ended. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>Date time offset when user lock is ended.</returns>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            return Task.FromResult<DateTimeOffset>(user.LockoutEndDate??DateTimeOffset.MinValue);
        }

        /// <summary>
        /// Increments access failed counter. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>Task.</returns>
        public Task<Int32> IncrementAccessFailedCountAsync(User user)
        {
            user.AccessFailedCount++;
            userRepository.Update(user);
            unitOfWork.CommitChanges();

            //better way is to return data after update 
            //update u
            //   set u.AccessFailedCount = AccessFailedCount + 1
            //output inserted.AccessFailedCount
            // where u.Id = @userId
            
            // it works much better and correctly when we can update the same attribute in the same time (selects on client may be have delays also) 
            return Task.FromResult<Int32>(user.AccessFailedCount); 
        }

        /// <summary>
        /// Resets access failedc counter. No implementation.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ResetAccessFailedCountAsync(User user)
        {
            user.AccessFailedCount = 0;
            userRepository.Update(user);
            unitOfWork.CommitChanges();
            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Enables user account. No implementation.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="enabled">If true then enable user account.</param>
        /// <returns>Task.</returns>
        public Task SetLockoutEnabledAsync(User user, Boolean enabled)
        {
            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Sets date time offset when user account is unlocked.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="lockoutEnd">Date time offset when user account is unlocked.</param>
        /// <returns>Task.</returns>
        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDate = lockoutEnd;
            userRepository.Update(user);
            unitOfWork.CommitChanges();
            return Task.FromResult<Object>(null);
        }
    }
}
