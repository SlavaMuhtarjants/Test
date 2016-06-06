namespace Weezlabs.Storgage.DataTransferObjects.Message
{        
    using System.ComponentModel.DataAnnotations;    

    /// <summary>
    /// Contact host request.
    /// </summary>
    public class ContactHostRequest : PostMessageRequest
    {
        /// <summary>
        /// Post offer request.
        /// </summary>
        [Required]
        public override PostOfferRequest Offer { get; set; }
    }
}
