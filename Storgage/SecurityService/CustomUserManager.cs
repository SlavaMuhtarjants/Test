namespace Weezlabs.Storgage.SecurityService
{
    using System;
    using System.Diagnostics.Contracts;

    using Model;
    using Model.Exceptions;
    using UtilService;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using System.Threading.Tasks;
    

    /// <summary>
    /// Custom user manager of ASP Identity..
    /// </summary>
    public class CustomUserManager : UserManager<User, Guid>
    {
        private readonly IAppSettings appSettings;
        //private readonly IUserRepository userRepository;
        //private readonly IDictionaryProvider dictionaryProvider;

        private readonly IUserLockoutStore<User, Guid> userLockoutStore;

        public CustomUserManager(IUserStore<User, Guid> store)
            : base(store)
        {
        }


        /// <summary>
        /// Create instance of user manager.
        /// </summary>
        /// <param name="userStore">User store.</param>
        /// <param name="appSettings"></param>
        public CustomUserManager(IUserStore<User, Guid> userStore, IAppSettings appSettings)
            : base(userStore)
        {
            Contract.Requires(appSettings != null);
            Contract.Requires(userStore != null);

            this.appSettings = appSettings;
            this.userLockoutStore = (IUserLockoutStore<User, Guid>)userStore;

            UserValidator = new UserValidator<User, Guid>(this)
            {
                RequireUniqueEmail = false,
                AllowOnlyAlphanumericUserNames = false
            };
            var dataProtectionProvider = DataProtectionProviderWrapper.DataProtectionProvider;
            UserTokenProvider = new DataProtectorTokenProvider<User, Guid>(dataProtectionProvider.Create("ASP.NET Identity"))
            {
                TokenLifespan = TimeSpan.FromHours(appSettings.GetSetting<Int32>("emailTokenExpiredTime"))
            };
            this.SmsService = IdentityMessageServiceResolver.GetSmsService();
            this.EmailService = IdentityMessageServiceResolver.GetEmailService();

            this.UserLockoutEnabledByDefault = appSettings.GetSetting<Boolean>("UserLockoutEnabledByDefault");
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(appSettings.GetSetting<Double>("DefaultAccountLockoutTimeSpan"));
            this.MaxFailedAccessAttemptsBeforeLockout = appSettings.GetSetting<Int32>("MaxFailedAccessAttemptsBeforeLockout");

            this.PasswordValidator = new PasswordValidator() { RequireDigit = true, RequireLowercase = true, RequireUppercase = true, RequireNonLetterOrDigit = false };
        }

        public override async Task<User> FindAsync(string userName, string password)
        {
            var user = await FindByNameAsync(userName);

            if (user == null) return null;

            if (user.LockoutEndDate >= DateTimeOffset.Now)
            {                                
                throw new AccessDeniedException(String.Format(Resources.Messages.UserIsBlockedTill, user.LockoutEndDate.ToString()));
            }

            if (user.LockoutEndDate <= DateTimeOffset.Now && user.AccessFailedCount >= this.MaxFailedAccessAttemptsBeforeLockout)
            {
                await userLockoutStore.ResetAccessFailedCountAsync(user);
            }

            var isPasswordValid = await CheckPasswordAsync(user, password);
            if (isPasswordValid)
            {
                if (user.AccessFailedCount > 0)
                {
                    await userLockoutStore.ResetAccessFailedCountAsync(user);
                }
                return user;
            }
            else
            {
                await IncrementAccessFailedCount(user);
                throw new AccessDeniedException(String.Format(Resources.Messages.UserIncorrectPassword, user.AccessFailedCount.ToString(), MaxFailedAccessAttemptsBeforeLockout.ToString()));
            }
        }

        private async Task IncrementAccessFailedCount(User user)
        {
            var accessFailedCount = await userLockoutStore.IncrementAccessFailedCountAsync(user);

            var shouldLockoutUser = accessFailedCount >= MaxFailedAccessAttemptsBeforeLockout;

            if (shouldLockoutUser)
            {
                var lockoutEndDate = new DateTimeOffset(DateTime.Now + DefaultAccountLockoutTimeSpan);
                await userLockoutStore.SetLockoutEndDateAsync(user, lockoutEndDate);
            }
        }

    }
}
