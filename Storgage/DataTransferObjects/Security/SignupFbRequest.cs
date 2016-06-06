namespace Weezlabs.Storgage.DataTransferObjects.Security
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Sign up FB request.
    /// </summary>
    public class SignupFbRequest
    {
        /// <summary>
        /// Fb token.
        /// </summary>
        [Required]
        public String FbToken { get; set; }
    }
}
