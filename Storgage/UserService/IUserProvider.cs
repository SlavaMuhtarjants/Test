namespace Weezlabs.Storgage.UserService
{
    using System;
    using DataTransferObjects.User;

    /// <summary>
    /// Interface for user provider.
    /// </summary>
    /// <typeparam name="TKey">Identifier type.</typeparam>
    public interface IUserProvider<TKey>
    {
        /// <summary>
        /// Returns short info for user.
        /// </summary>        
        /// <param name="userId">user identifier.</param>
        /// <returns>Short info about user.</returns>
        UserInfo GetUserShortInfo(TKey userId);

        Boolean UserDel(String userXml);
    }
}
