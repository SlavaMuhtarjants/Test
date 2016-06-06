namespace Weezlabs.Storgage.DataTransferObjects.User
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Contains user contact info.
    /// </summary>    
    public class UserContact
    {    
        /// <summary>
        /// User email.
        /// </summary>
        [StringLength(256)]     
        [EmailAddress]
        public String Email { get; set; }

        /// <summary>
        /// User phone.
        /// </summary>
        [StringLength(15)]
        [RegularExpression(@"^\+[0-9]{10,15}$")]
        public String Phone { get; set; }
    }
}
