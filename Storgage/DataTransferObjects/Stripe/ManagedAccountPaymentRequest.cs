namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Object for add bank account/debit card response
    /// </summary>
    public class ManagedAccountPaymentRequest
    {
        /// <summary>
        /// Bank account/debit card token
        /// </summary>
        [Required]
        public String Token { get; set; }
    }
}
