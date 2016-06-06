namespace Weezlabs.Storgage.DataTransferObjects.User
{
    using System;
    using System.Diagnostics.Contracts;

    using DataLayer.Dictionaries;

    /// <summary>
    /// Contains public information about user.
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// User identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User full name.
        /// </summary>
        public UserFullName FullName { get; set; }

        /// <summary>
        /// User avatar on S3 bucket.
        /// </summary>
        public UserAvatar Avatar { get; set; } 

        /// <summary>
        /// If it is true, then user phone was verified with success.
        /// </summary>
        public Boolean IsValidPhoneNumber { get; set; }

        /// <summary>
        /// If it is true, then email was verified with success.
        /// </summary>
        public Boolean IsValidEmail { get; set; }

        /// <summary>
        /// User total rating (average).
        /// </summary>
        public Double Rating { get; set; }

        /// <summary>
        /// amount of rates for user
        /// </summary>
        public Double CountRating { get; set; }

        /// <summary>
        /// User was created based on external system.
        /// For now it's Facebook onle
        /// </summary>
        public Boolean IsExternal { get; set; }

        /// <summary>
        /// Returns true if FB verified.
        /// </summary>
        public Boolean IsFbVerified { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserInfo()
        {
        }

        /// <summary>
        /// Creates public info about user from model.
        /// </summary>
        /// <param name="user">Model object for user.</param>
        public UserInfo(Model.User user)
        {
            Contract.Requires(user != null);
            Id = user.Id;
            FullName = new UserFullName 
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
            };           

            IsValidPhoneNumber = user.PhoneVerificationStatusID == Model.Enums.PhoneVerificationStatus.Verified.GetDictionaryId();
            IsValidEmail = user.EmailVerificationStatusID == Model.Enums.EmailVerificationStatus.Verified.GetDictionaryId();
            Avatar = new UserAvatar(user.AvatarLink);
            Rating = user.AvgRate.HasValue ? (Double)user.AvgRate.Value : 0.0;
            CountRating = user.CountRating;
            IsExternal = user.IsExternal;
            IsFbVerified = !String.IsNullOrWhiteSpace(user.FacebookID);
        }
    }
}