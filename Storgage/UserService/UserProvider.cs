namespace Weezlabs.Storgage.UserService
{
    using System;
    using System.Diagnostics.Contracts;

    using DataLayer.Users;
    using DataTransferObjects.User;    

    /// <summary>
    /// User info provider.
    /// </summary>
    public class UserProvider : IUserProvider<Guid>
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Create instance of repository.
        /// </summary>
        /// <param name="userRepository">User repository.</param>
        public UserProvider(IUserRepository userRepository)
        {
            Contract.Requires(userRepository != null);

            this.userRepository = userRepository;
        }

        /// <summary>
        /// Returns short info about user.
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Short info</returns>
        public UserInfo GetUserShortInfo(Guid userId)
        {
            var user = userRepository.GetById(userId);
            if (user == null)
            {
                return null;
            }

            return new UserInfo(user);
        }

        public Boolean UserDel(String userXml)
        {
            return userRepository.UserDel(userXml);
        }
    }
}
