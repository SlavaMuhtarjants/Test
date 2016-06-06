namespace Weezlabs.Storgage.DataTransferObjects.Message
{    
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;

    using Model.Enums;
    using User;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Get last message response request.
    /// </summary>
    public class GetMessageResponse
    {
        /// <summary>
        /// Message identifier.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Message info.
        /// </summary>        
        public MessageInfo Message { get; set; }

        [Required]
        /// <summary>
        /// Sender info.
        /// </summary>
        public UserInfo Sender { get; set; }

        /// <summary>
        /// Message status.
        /// </summary>
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageDeliveredStatus MessageDeliveredStatus { get; set; }


        /// <summary>
        /// Offer info.
        /// </summary>        
        public GetOfferResponse Offer { get; set; }

        /// <summary>
        /// Sent date.
        /// </summary>
        public DateTimeOffset SentDate { get; set; }

        /// <summary>
        /// Delivered date.
        /// </summary>
        public DateTimeOffset? DeliveredDate { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetMessageResponse()
        {
        }

        /// <summary>
        /// Creates response using model.
        /// </summary>
        /// <param name="message">Message from model.</param>
        public GetMessageResponse(Model.Message message)
        {
            Contract.Requires(message != null);

            Id = message.Id;           
            Message = String.IsNullOrWhiteSpace(message.MessageText) 
                ? null   
                : new MessageInfo
                {
                    Text = message.MessageText,                
                };

            SentDate = message.ReceivedDate;
            DeliveredDate = message.SentDate;
            MessageDeliveredStatus = message.MessageDeliveredStatu.ToEnum();
            Offer = message.MessageOffer == null ? null : new GetOfferResponse(message.MessageOffer);
            Sender = message.MessageOffer != null && message.MessageOffer.CanDisplayUserContactInfo() 
                ? new UserFullInfo(message.User) 
                : new UserInfo(message.User);
        }
    }
}
