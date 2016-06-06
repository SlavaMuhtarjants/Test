namespace Weezlabs.Storgage.RestApi.Tasks.OfferExpiration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using DataLayer;
    using DataLayer.ChatAndMessages;
    using DataLayer.Dictionaries;
    using Model;   

    /// <summary>
    /// Offer expiration processor.
    /// </summary>
    internal class OfferExpirationDataProvider
    {
        /// <summary>
        /// Expires offers.
        /// </summary>
        /// <param name="recordsCount">Record counts to process.</param>
        /// <param name="offerRepository">Message Offer Repository.</param>
        /// <param name="offerHistoryRepository">Offer History Repository.</param>
        /// <param name="dictionaryProvider">Dictionary Provider.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <returns>Expired offers.</returns>
        internal static IEnumerable<ExpiredOfferInfo> ExpireOffers(Int32 recordsCount, 
            IOfferRepository offerRepository,
            IOfferHistoryRepository offerHistoryRepository, 
            IDictionaryProvider dictionaryProvider,
            IUnitOfWork unitOfWork)
        {                  
            DateTime dateTimeNow = DateTime.UtcNow;
            var pendingStatusId = Model.Enums.MessageOfferStatus.Pending.GetDictionaryId();
            IEnumerable<ExpiredOfferInfo> expiredOffers = offerRepository.GetAll() 
                .Include(o => o.Message.Chat)               
                .Where(o => o.RentSince < dateTimeNow 
                && o.MessageOfferHistory != null 
                && o.MessageOfferHistory.StatusId == pendingStatusId
                && o.Message.Chat.LastMessageOfferId == o.Id)
                .Take(recordsCount).ToArray().Select(o => new ExpiredOfferInfo
                {
                    OfferID = o.Id,
                    UserID = o.Message.UserId,
                    ChatID = o.Message.ChatId
                });

            if (expiredOffers.Count() == 0)
            {
                return expiredOffers;
            }

            IEnumerable<MessageOfferHistory> offersHistory = expiredOffers.Select(o => new MessageOfferHistory
            {
                MessageOfferId = o.OfferID,
                UserId = o.UserID,
                StatusId = Model.Enums.MessageOfferStatus.Expired.GetDictionaryId(),
                ChangedStatusDate = dateTimeNow
            });

            
            offerHistoryRepository.AddRange(offersHistory);
            unitOfWork.CommitChanges();

            return expiredOffers;
        }
    }
}
