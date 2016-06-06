namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System;

    /// <summary>
    /// Stripe customer bank account.
    /// </summary>
    public class StripeBankAccountInfo
    {
        /// <summary>
        /// Stripe bank account full info (just for getting back when creating new bank account)
        /// </summary>
        public StripeBankAccountItem StripeBankAccount { get; set; }

        /// <summary>
        /// Customer's bank account identifier
        /// </summary>
        public String StripeBankAccountId { get; set; }

        /// <summary>
        /// Customer's bank account token
        /// </summary>
        public String StripeBankAccountToken { get; set; }
    }
}
