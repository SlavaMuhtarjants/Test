namespace Weezlabs.Storgage.DataTransferObjects.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// View model for reset password page.
    /// </summary>
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = @"Email")]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(30)]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = @"Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = @"Confirm password")]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Messages),
            ErrorMessageResourceName = "InvalidPasswordConfirmation")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
