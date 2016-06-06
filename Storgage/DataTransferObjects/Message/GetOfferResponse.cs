namespace Weezlabs.Storgage.DataTransferObjects.Message
{
    using System;
    using System.Collections.Generic;    
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Get offer response.
    /// </summary>
    public class GetOfferResponse : Offer
    {               
        /// <summary>
        /// Offer identifier.
        /// </summary>
        public Guid Id { get; set; }
      

        /// <summary>
        /// Date time posted.
        /// </summary>
        public DateTimeOffset PostedAt { get; set; }        

        /// <summary>
        /// Current offer state.
        /// </summary>
        public OfferState CurrentState { get; set; }

        /// <summary>
        /// Date when offer will be stopped.
        /// </summary>
        public DateTimeOffset? StopAt { get; set; }

        /// <summary>
        /// Offer states.
        /// </summary>
        public IEnumerable<OfferState> States { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetOfferResponse()
        {
        }

        /// <summary>
        /// Creates get offer response from model.
        /// </summary>
        /// <param name="offer">Model.</param>
        public GetOfferResponse(Model.MessageOffer offer)
            : base(offer)
        {
            Contract.Requires(offer != null);

            Id = offer.Id;           
            PostedAt = offer.Message.ReceivedDate;
            CurrentState = new OfferState(offer.MessageOfferHistory);
            States = offer.MessageOfferHistories.Select(x => new OfferState(x));
            StopAt = offer.StopAt;
        }
    }
}
