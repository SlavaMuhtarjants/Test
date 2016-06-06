namespace Weezlabs.Storgage.DataTransferObjects.Booking
{
    using System;
    using Weezlabs.Storgage.DataTransferObjects.Message;
    using Weezlabs.Storgage.DataTransferObjects.Space;
    using Weezlabs.Storgage.DataTransferObjects.User;

    public class MyBookingResponse
    {
        public GetSpaceResponse Space { get; set; }
        public UserFullInfo Owner { get; set; }
        public GetOfferResponse LastMessageOffer { get; set; }
        public Guid ChatId { get; set; }
        public OfferState ApprovedMessageOfferHistory { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MyBookingResponse()
        {

        }
    }
}
