namespace Weezlabs.Storgage.SecurityService
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using DataTransferObjects.Security;
    using Model;
    using DataTransferObjects.User;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Interface for authentication.
    /// </summary>
    public interface IAuthProvider
    {
        /// <summary>
        /// Signs user up.
        /// </summary>
        /// <param name="userModel">Sign up info.</param>
        /// <returns>Sign up result.</returns>
        Task<IdentityResult> RegisterUser(SignupInfo userModel);

        /// <summary>
        /// Signs user up by fackebook token.
        /// </summary>
        /// <param name="facebookToken">Facebook token with permissions.</param>
        /// <returns>Sign up result.</returns>
        Task<IdentityResultFb> RegisterFacebookUser(String facebookToken);

        /// <summary>
        /// Set facebookId to existing user without additional updates/refresh and etc.
        /// </summary>
        /// <param name="facebookToken">Facebook token with permissions.</param>
        /// <returns>Boolean</returns>
        Task<Boolean> SetFacebookUser(String facebookToken, Guid userId);

        /// <summary>
        /// Get user by identifier.
        /// </summary>
        /// <param name="userId">Identifier of user.</param>
        /// <returns>User.</returns>
        Task<User> GetUser(Guid userId);

            /// <summary>
        /// Finds user by username and password.
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Pasword.</param>
        /// <returns>User.</returns>
        Task<User> FindUser(String userName, String password);

        /// <summary>
        /// Finds user by facebookid.
        /// </summary>
        /// <param name="facebookId">facebook identifier</param>
        /// <returns>User.</returns>
        Task<User> FindUserByFb(String facebookId);

        /// <summary>
        /// Updates user's attributes Email, Phone, LastName, FirstName
        /// </summary>
        /// <param name="userId">User identifer</param>
        /// <param name="userToUpdate">Updated model but Email, Phone, LastName, FirstName attributes will be used only</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Current user with all attrubutes but Email, Phone, LastName, FirstName will be the same that were passed in the userToUpdate</returns>
        UserFullInfo UpdateUser(Guid userId, AccountInfo userToUpdate, Guid actorId);

        /// <summary>
        /// Change password for user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="request">Change password information</param>
        void ChangePassword(Guid userId, ChangePasswordInfo request);

        /// <summary>
        /// Reset password by email
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="token">Token.</param>
        /// <param name="password">Password.</param>
        Task<IdentityResult> ResetPassword(String email, String token, String password);

        /// <summary>
        /// Confirm email
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="token">Token.</param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> ConfirmEmail(Guid userId, String token);

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body.</param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> SendEmail(Guid userId, String subject, String body);

        /// <summary>
        /// Send phone confirmation message
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> SendPhoneConfirmation(Guid userId);

        /// <summary>
        /// Confirm phone
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="code">Token.</param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> ConfirmPhone(Guid userId, String code);

        /// <summary>
        /// Upload avatar
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="formData">IEnumerable of files.</param>
        /// <returns>Updated user</returns>
        UserAvatar UploadAvatar(Guid userId, IEnumerable<HttpContent> formData);

        /// <summary>
        /// Skip avatar.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        void SkipAvatar(Guid userId);

        /// <summary>
        /// Add refresh token
        /// </summary>
        /// <param name="token">Refresh token model.</param>
        /// <returns></returns>
        Task<Boolean> AddRefreshToken(RefreshToken token);

        /// <summary>
        /// Remove refresh token.
        /// </summary>
        /// <param name="refreshTokenId">Refresh token identifier.</param>
        /// <param name="actor"></param>
        /// <returns></returns>
        Task<Boolean> RemoveRefreshToken(String refreshTokenId, Guid actor = default(Guid));

        /// <summary>
        /// Find refresh token by id.
        /// </summary>
        /// <param name="refreshTokenId">Refresh token identifier.</param>
        /// <returns>RefreshToken</returns>
        Task<RefreshToken> FindRefreshToken(String refreshTokenId);
    }
}
