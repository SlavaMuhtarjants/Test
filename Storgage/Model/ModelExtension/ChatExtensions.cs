namespace Weezlabs.Storgage.Model
{
    using System;
    using System.Linq;
    using System.Data.Entity;

    using Exceptions;

    public static class ChatExtensions
    {
        public static IQueryable<Chat> AttachIncludes(this IQueryable<Chat> chats)
        {
            return chats.Include(c => c.User).Include(c => c.Space).Include(c => c.Messages).Include(c => c.User.PhoneVerificationStatus).
                Include(c => c.User.EmailVerificationStatus).Include(c => c.Space.PhotoLibraries).Include(c => c.Space.SizeType).
                Include(c => c.Space.SpaceAccessType).Include(c => c.Space.SpaceType).Include(c => c.Space.Zip).
                Include(c => c.Space.User.PhoneVerificationStatus).
                Include(c => c.Messages.Select(m => m.MessageOffer)).Include(c => c.Messages.Select(m => m.MessageOffer.RentPeriodType)).
                Include(c => c.Messages.Select(m => m.MessageOffer.MessageOfferHistories.Select(moh => moh.MessageOfferStatus))).
                Include(c => c.Messages.Select(m => m.MessageOffer.MessageOfferHistories)).Include(c => c.ChatMembers).
                Include(c => c.ApprovedMessageOfferHistory.Rating);
        }

        public static Chat GetChatWithCurrentOffer(this IQueryable<Chat> chats, Guid chatId, Guid actorId)
        {
            var chat = chats.Where(x => x.Id == chatId)
                .Include(x => x.LastMessageOffer.MessageOfferHistory)
                .Include(x => x.LastMessageOffer.Message)
                .Include(x => x.ChatMembers)
                .SingleOrDefault();
            if (chat == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.ChatNotFound, chatId));
            }          

            return chat;
        }
    }
}