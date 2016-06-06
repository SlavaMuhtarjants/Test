namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System.Collections.Generic;

    using Model;

    /// <summary>
    /// Dictionary provider.
    /// </summary>
    public interface IDictionaryProvider
    {
        /// <summary>
        /// Space types.
        /// </summary>
        IEnumerable<SpaceType> SpaceTypes { get; }
        
        /// <summary>
        /// Space access types.
        /// </summary>
        IEnumerable<SpaceAccessType> SpaceAccessTypes { get; }
        
        /// <summary>
        /// Size types.
        /// </summary>
        IEnumerable<SizeType> SizeTypes { get; }
        
        /// <summary>
        /// Phone verification statuses.
        /// </summary>
        IEnumerable<PhoneVerificationStatus> PhoneVerificationStatuses { get; }
        
        /// <summary>
        /// Email verification statuses.
        /// </summary>
        IEnumerable<EmailVerificationStatus> EmailVerificationStatuses { get; }

        /// <summary>
        /// Message Delivered Statuses.
        /// </summary>
        IEnumerable<MessageDeliveredStatus> MessageDeliveredStatuses { get; }

        /// <summary>
        /// Message offer statuses.
        /// </summary>
        IEnumerable<MessageOfferStatus> MessageOfferStatuses { get; }

        /// <summary>
        /// Rent period types.
        /// </summary>
        IEnumerable<RentPeriodType> RentPeriodTypes { get; }

        /// <summary>
        /// Abuser Type Dictionary
        /// </summary>
        IEnumerable<AbuseTypeDictionary> AbuseTypeDictionary { get; }
    }
}
