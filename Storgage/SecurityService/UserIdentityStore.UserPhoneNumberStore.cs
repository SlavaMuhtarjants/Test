namespace Weezlabs.Storgage.SecurityService
{    
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    using Model;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface implementation of ASP Identity to manage by phone number.
    /// </summary>
    public partial class UserIdentityStore : IUserPhoneNumberStore<User, Guid>
    {
        /// <summary>
        /// Returns user phone number.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>User phone number.</returns>
        public Task<String> GetPhoneNumberAsync(User user)
        {
            Contract.Requires(user != null);

            return Task.FromResult<String>(user.Phone);
        }

        /// <summary>
        /// Returns status that user phone is verified.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>If true them user phone is confirmed.</returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(User user)
        {
            Contract.Requires(user != null);

            return Task.FromResult<Boolean>(user.PhoneVerificationStatus.ToEnum() == Model.Enums.PhoneVerificationStatus.Verified);
        }

        /// <summary>
        /// Sets up phone number.
        /// </summary>
        /// <param name="user">User phonne number.</param>
        /// <param name="phoneNumber"></param>
        /// <returns>Task.</returns>
        public Task SetPhoneNumberAsync(User user, String phoneNumber)
        {
            Contract.Requires(user != null);

            user.Phone = phoneNumber;

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Sets up status of phone number verification.
        /// </summary>
        /// <param name="user">USer.</param>
        /// <param name="confirmed">True if phone number was confirmed.</param>
        /// <returns>Task.</returns>
        public Task SetPhoneNumberConfirmedAsync(User user, Boolean confirmed)
        {
            Contract.Requires(user != null);

            user.PhoneVerificationStatus = confirmed
                ? dictionaryProvider.PhoneVerificationStatuses
                    .Single(x => x.ToEnum() == Model.Enums.PhoneVerificationStatus.Verified)
                : dictionaryProvider.PhoneVerificationStatuses
                    .Single(x => x.ToEnum() == Model.Enums.PhoneVerificationStatus.NotVerified);
                    

            return Task.FromResult<Object>(null);
        }
    }
}
