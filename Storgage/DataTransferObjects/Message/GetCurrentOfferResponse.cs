namespace Weezlabs.Storgage.DataTransferObjects.Message
{    
    using User;

    /// <summary>
    /// Get current offer response.
    /// </summary>
    public class GetCurrentOfferResponse : GetOfferResponse
    {
        /// <summary>
        /// Sender of current offer.
        /// </summary>
        public UserInfo Sender { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetCurrentOfferResponse()
        {
        }

        /// <summary>
        /// Creates DTO using model.
        /// </summary>
        /// <param name="offer">Model object.</param>
        public GetCurrentOfferResponse(Model.MessageOffer offer)
            : base(offer)
        {
            Sender =  offer.CanDisplayUserContactInfo() ? new UserFullInfo(offer.Message.User) : new UserInfo(offer.Message.User);
        }
    }
}
