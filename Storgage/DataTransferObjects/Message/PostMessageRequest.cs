namespace Weezlabs.Storgage.DataTransferObjects.Message
{
    using System;
    using System.ComponentModel.DataAnnotations;    

    /// <summary>
    /// Post message request.
    /// </summary>
    public class PostMessageRequest
    {
        /// <summary>
        /// Sender identifier.
        /// </summary>
        [Required]
        public Guid SenderId { get; set; }    

        /// <summary>
        /// Message.
        /// </summary>                  
        public virtual MessageInfo Message { get; set; }

        /// <summary>
        /// Offer.
        /// </summary>       
        public virtual PostOfferRequest Offer { get; set; }       
    }
}
