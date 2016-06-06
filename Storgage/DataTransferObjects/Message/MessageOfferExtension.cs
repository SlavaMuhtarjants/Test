namespace Weezlabs.Storgage.DataTransferObjects.Message
{
    using System;
    using System.Diagnostics.Contracts;

    using DataLayer.Dictionaries;
    using Model;   

    public static class MessageOfferExtension
    {
        /// <summary>
        /// Returns true if we can display user contact info.
        /// </summary>
        public static Boolean CanDisplayUserContactInfo(this MessageOffer messageOffer)
        {
            Contract.Requires(messageOffer != null);

            return  messageOffer.RentSince < DateTimeOffset.Now
                    && (messageOffer.MessageOfferHistory.StatusId == Model.Enums.MessageOfferStatus.Approved.GetDictionaryId()
                    || messageOffer.MessageOfferHistory.StatusId == Model.Enums.MessageOfferStatus.Stopped.GetDictionaryId());
        }
    }
}
