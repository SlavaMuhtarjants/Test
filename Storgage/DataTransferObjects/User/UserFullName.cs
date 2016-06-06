namespace Weezlabs.Storgage.DataTransferObjects.User
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// User full name.
    /// </summary>
    public class UserFullName
    {
        /// <summary>
        /// User firstname.
        /// </summary>
        [Required]
        [MaxLength(30)]
        public String Firstname { get; set; }

        /// <summary>
        /// User Lastname.
        /// </summary>
        [Required]
        [MaxLength(30)]
        public String Lastname { get; set; }
    }
}
