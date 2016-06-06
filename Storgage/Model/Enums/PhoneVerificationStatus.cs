namespace Weezlabs.Storgage.Model.Enums
{
    /// <summary>
    /// Phone verification statuses.
    /// </summary>
    public enum PhoneVerificationStatus
    {
        /// <summary>
        /// Phone number has not verified yet.
        /// </summary>        
        MustVerified,
        
        /// <summary>
        /// Phone number was verified and phone number is invalid.
        /// </summary>
        NotVerified,

        /// <summary>
        /// Phone number was verified and phone number is valid.
        /// </summary>
        Verified
    }
}
