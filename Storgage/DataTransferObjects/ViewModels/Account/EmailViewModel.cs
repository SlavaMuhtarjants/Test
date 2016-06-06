namespace Weezlabs.Storgage.DataTransferObjects.ViewModels.Account
{
    using System;

    /// <summary>
    /// Model for sending email for reset password.
    /// </summary>
    public abstract class EmailViewModel
    {
        /// <summary>
        /// User email
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Link to reset password
        /// </summary>
        public String Link { get; set; }

        /// <summary>
        /// Expired date
        /// </summary>
        public DateTime Expired { get; set; }
    }
}
