namespace Weezlabs.Storgage.DataTransferObjects.User
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// User Full info + token.
    /// </summary>
    public class UserFullInfoFb : UserFullInfo
    {
        /// <summary>
        /// Creates user full info.
        /// </summary>
        /// <param name="user">User from model.</param>
        public UserFullInfoFb(Model.User user)
            : base(user)
        {
        }

        /// <summary>
        /// Access token.
        /// </summary>
        public String AccessToken { get; set; }

        /// <summary>
        /// Access token type.
        /// </summary>
        public String AccessTokenType { get; set; }

        /// <summary>
        /// Refresh token.
        /// </summary>
        public String RefreshToken { get; set; }

        /// <summary>
        /// When token will be expired.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
