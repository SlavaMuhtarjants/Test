namespace Weezlabs.Storgage.DataTransferObjects.User
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// User info + user contact info
    /// </summary>
    public class UserFullInfo : UserInfo
    {
        /// <summary>
        /// User contact info.
        /// </summary>
        public UserContact Contact { get; set; }

        /// <summary>
        /// Facebook identifier 
        /// </summary>
        [MaxLength(64)]
        public String FacebookId { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserFullInfo()
        {
        }

        /// <summary>
        /// Create instance using model object.
        /// </summary>
        /// <param name="user">Model object.</param>
        public UserFullInfo(Model.User user)
            : base(user)
        {
            Contact = new UserContact
            {
                Email = user.Email,
                Phone = user.Phone
            };

            this.FacebookId = user.FacebookID;
        }
    }
}
