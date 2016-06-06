namespace Weezlabs.Storgage.DataTransferObjects.Security
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    /// <summary>
    /// Contains information about user signup.
    /// </summary>
    public class SignupInfo : AccountInfo
    {
        /// <summary>
        /// User password.
        /// </summary>
        [Required]
        [MaxLength(PasswordRules.MaxLength)]
        [MinLength(PasswordRules.MinLength)]
        public String Password { get; set; }

        public Model.User ToModel()
        {
            return new Model.User
            {
                Firstname = this.FullName.Firstname,
                Lastname = this.FullName.Lastname,
                Email = this.Contact.Email,
                Phone = this.Contact.Phone,
            };
        }

    }
}