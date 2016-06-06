namespace Weezlabs.Storgage.DataTransferObjects.Security
{
    using System.ComponentModel.DataAnnotations;
    using Weezlabs.Storgage.DataTransferObjects.User;

    public class AccountInfo
    {
        /// <summary>
        /// User Full Name
        /// </summary>
        [Required]
        public UserFullName FullName { get; set; }
        /// <summary>
        /// User Contact
        /// </summary>
        [Required]
        public UserContact Contact { get; set; }
    }
}
