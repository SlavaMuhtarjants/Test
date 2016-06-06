namespace Weezlabs.Storgage.DataTransferObjects.Security
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Contains information about old and new pssaword.
    /// </summary>
    public class ChangePasswordInfo
    {
        /// <summary>
        /// New user password.
        /// </summary>
        [Required]
        [MinLength(PasswordRules.MinLength)]
        [MaxLength(PasswordRules.MaxLength)]
        public String NewPassword { get; set; }

        /// <summary>
        /// Old user password.
        /// </summary>
        [Required]
        [MinLength(PasswordRules.MinLength)]
        [MaxLength(PasswordRules.MaxLength)]
        public String OldPassword { get; set; }
    }
}