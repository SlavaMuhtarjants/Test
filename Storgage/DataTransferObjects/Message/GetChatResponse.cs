namespace Weezlabs.Storgage.DataTransferObjects.Message
{
    using System;
    using System.Diagnostics.Contracts;

    using Space;
    using User;
    using Weezlabs.Storgage.DataTransferObjects.Rating;    

    /// <summary>
    /// Get chat response.
    /// </summary>
    public class GetChatResponse
    {
        /// <summary>
        /// Chat Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User info about creator.
        /// </summary>
        public UserInfo Creator { get; set; }

        /// <summary>
        /// Space info.
        /// </summary>
        public GetSpaceResponse Space { get; set; }

        /// <summary>
        /// User's unreaded messages count in this chat.
        /// </summary>
        public Int32 YourUnreadedMessage { get; set; }    

        /// <summary>
        /// Current offer.
        /// </summary>
        public GetCurrentOfferResponse CurrentOffer { get; set; }

        /// <summary>
        /// Message Offer History that was approved
        /// this parameter MUST be passed via {} to prevent redundant DB calls
        /// </summary>
        public OfferState ApprovedMessageOfferHistory;

        /// <summary>
        /// Chat may have rating 
        /// </summary>

        public RatingInfo ChatRating { get; set; }

        /// <summary>
        /// Date when there was last message or offer state was changed
        /// </summary>
        public DateTimeOffset LastModified { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetChatResponse()
        { }

        /// <summary>
        /// Creates response DTO from model.
        /// </summary>
        /// <param name="chat">Chat from model.</param>
        public GetChatResponse(
            Model.Chat chat                            
            )
        {
            Contract.Requires(chat != null);

            Id = chat.Id;
            var displayContactInfo = chat.LastMessageOffer != null && chat.LastMessageOffer.RentSince < DateTimeOffset.Now;
            Creator = (displayContactInfo) ? new UserFullInfo(chat.User) : new UserInfo(chat.User);           
            Space = new GetSpaceResponse(chat.Space, displayContactInfo);
            if (chat.LastMessageOfferId != null)
            {
                CurrentOffer = new GetCurrentOfferResponse(chat.LastMessageOffer);
            }

            if (chat.ApprovedMessageOfferHistory != null && chat.ApprovedMessageOfferHistory.Rating != null)
            {                
                //Possible we need to pass user here also
                ChatRating = new RatingInfo (chat.ApprovedMessageOfferHistory.Rating);
            }

            LastModified = chat.LastModified;

        }
    }
}
