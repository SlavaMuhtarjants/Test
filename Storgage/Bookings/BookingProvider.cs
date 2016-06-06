namespace Weezlabs.Storgage.Bookings
{
    using DataLayer.ChatAndMessages;
    using DataLayer.Users;
    using DataTransferObjects.Message;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Weezlabs.Storgage.DataTransferObjects.Booking;
    using Weezlabs.Storgage.DataTransferObjects.Space;
    using Weezlabs.Storgage.DataTransferObjects.User;

    public class BookingProvider : IBookingProvider
    {
        private readonly IChatRepository chatRepository;
        private readonly IUserReadonlyRepository userRepository;


        public BookingProvider(
            IChatRepository chatRepository,
            IUserReadonlyRepository userRepository)
        {
            Contract.Requires(chatRepository != null);
            Contract.Requires(userRepository != null);

            this.chatRepository = chatRepository;
            this.userRepository = userRepository;
        }

        public List<MyBookingResponse> GetMyBookings(Guid userId)
        {

            var chats = chatRepository
                .GetAll().Where(x => x.UserId == userId && x.ApprovedMessageOfferHistoryId != null)
                .Include(x => x.Space)
                .Include(x => x.ApprovedMessageOfferHistory)
                .Include(x => x.User);

            var spaceQuery = (
                from u in userRepository.GetAll()
                from ch in chats.Where(x => x.UserId == u.Id && x.ApprovedMessageOfferHistoryId != null).DefaultIfEmpty()
                where u.Id == userId
                select new
                {
                    user = u,
                    chat = ch,
                    space = ch.Space,
                    approvedMessageOfferHistory = ch.ApprovedMessageOfferHistory,
                    lastMessageOffer = ch.LastMessageOffer,
                    spaceOwner = ch.User
                });

            //Linq2Entities supports ONLY parameter less constructors 
            //We can try to use x.space and x.space in {}
            //or map it compleatelly 
            var firstList = spaceQuery.ToList(); //This quuery doesn call redundant queries in cycles

            if (firstList.Count == 0)
            {
                return null;
            }

            //use the 1st user
            //it will call 
            //IsValidPhoneNumber = user.PhoneVerificationStatus.ToEnum() == Model.Enums.PhoneVerificationStatus.Verified;
            //IsValidEmail = user.EmailVerificationStatus.ToEnum() == Model.Enums.EmailVerificationStatus.Verified;
            //separaelly

            List<MyBookingResponse> resultNew = null;

            // it will call 
            // Photo = space.PhotoLibraries.OrderBy(x => x.Id).Select(x => new Photo(x)); -- IT IS LIST !!!
            // ZipCode = space.Zip == null? (String)null: space.Zip.ZipCode;
            //...
            // Also it calls Owner = new UserInfo(space.User); that calls UserInfo creation with the same problem as "var result = new GetUserChatList(firstList[0].user);"
            if (firstList[0].space != null)
            {
                // Important !!!
                // Here I pass instances to MyBookingResponse ONLY
                // If OfferState or GetCurrentOfferResponse calls database when User is needed again - it will continue to call it

                resultNew = firstList.Select(
                    x => new MyBookingResponse()
                    {
                        Space = new GetSpaceResponse(x.space, x.lastMessageOffer.RentSince < DateTimeOffset.Now),
                        Owner = new UserFullInfo(x.spaceOwner),
                        LastMessageOffer = new GetOfferResponse(x.lastMessageOffer),
                        ChatId = x.chat.Id,
                        ApprovedMessageOfferHistory = new OfferState(x.approvedMessageOfferHistory)
                    }
                    ).ToList();
            }
            else
            {
                resultNew = new List<MyBookingResponse>(new MyBookingResponse[0]);
            }
            return resultNew;
        }
    }
}
