namespace Weezlabs.Storgage.Model.Enums
{
    /// <summary>
    /// Email verification statuses
    /// </summary>
    public enum EmailVerificationStatus
    {
        /// <summary>
        /// Email has not verified yet.
        /// </summary>        
        MustVerified,

        /// <summary>
        /// Email was verified and it is invalid.
        /// </summary>
        NotVerified,

        /// <summary>
        /// Email was verified and it is valid.
        /// </summary>
        Verified
    }
}
