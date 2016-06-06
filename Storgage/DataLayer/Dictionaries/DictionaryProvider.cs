namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Caching;
    using Model;

    /// <summary>
    /// Dictionaries provider.
    /// </summary>
    public class DictionaryProvider : IDictionaryProvider
    {
        /// <summary>
        /// Space types.
        /// </summary>
        public IEnumerable<SpaceType> SpaceTypes
        {
            get
            {
                return FetchCollection("spaceTypes", spaceTypeRepository.GetAll);
            }
        }

        /// <summary>
        /// paymentPointTypeRepository.
        /// </summary>
        public IEnumerable<PaymentPointType> PaymentPointType
        {
            get
            {
                return FetchCollection("paymentPointType", paymentPointTypeRepository.GetAll);
            }
        }

        /// <summary>
        /// abuseTypeDictionaryRepository.
        /// </summary>
        public IEnumerable<AbuseTypeDictionary> AbuseTypeDictionary
        {
            get
            {
                return FetchCollection("abuseTypeDictionary", abuseTypeDictionaryRepository.GetAll);
            }
        }

        /// <summary>
        /// Space access types.
        /// </summary>
        public IEnumerable<SpaceAccessType> SpaceAccessTypes
        {
            get
            {
                return FetchCollection("spaceAccessTypes", spaceAccessTypeRepository.GetAll);
            }
        }

        /// <summary>
        /// Size types.
        /// </summary>
        public IEnumerable<SizeType> SizeTypes
        {
            get
            {
                return FetchCollection("sizeTypes", sizeTypeRepository.GetAll);
            }
        }

        /// <summary>
        /// Phone verification statuses.
        /// </summary>
        public IEnumerable<PhoneVerificationStatus> PhoneVerificationStatuses
        {
            get
            {
                return FetchCollection("phoneVerificationStatuses", phoneVerificationStatusRepository.GetAll);
            }
        }
        
        /// <summary>
        /// Email verification statuses.
        /// </summary>
        public IEnumerable<EmailVerificationStatus> EmailVerificationStatuses
        {
            get
            {
                return FetchCollection("emailVerificationStatuses", emailVerificationStatusRepository.GetAll);
            }
        }

        /// <summary>
        /// Size types.
        /// </summary>
        public IEnumerable<MessageDeliveredStatus> MessageDeliveredStatuses
        {
            get
            {
                return FetchCollection("messageDeliveredStatuses", messageDeliveredStatusRepository.GetAll);
            }
        }

        /// <summary>
        /// Message offer statuses.
        /// </summary>
        public IEnumerable<MessageOfferStatus> MessageOfferStatuses
        {
            get
            {
                return FetchCollection("messageOfferStatuses", messageOfferStatusRepository.GetAll);
            }
        }

        /// <summary>
        /// Rent period types.
        /// </summary>
        public IEnumerable<RentPeriodType> RentPeriodTypes
        {
            get
            {
                return FetchCollection("rentPeriodTypes", rentPeriodTypeRepository.GetAll);
            }
        }

        /// <summary>
        /// Creates instance of Dictionary provider.
        /// </summary>
        /// <param name="sizeTypeRepository">Size type repository.</param>
        /// <param name="spaceAccessTypeRepository">Space access type repository.</param>
        /// <param name="spaceTypeRepository">Space type repository.</param>
        /// <param name="phoneVerificationStatusRepository">Phone verification status repository.</param>
        /// <param name="emailVerificationStatusRepository">Email verification status repository.</param>
        /// <param name="messageDeliveredStatusRepository">Message delivered status repository.</param>
        /// <param name="messageOfferStatusRepository">Message offer status repository.</param>
        /// <param name="rentPeriodTypeRepository">Rent period type repository.</param>
        /// <param name="paymentPointTypeRepository">Payment Point Type repository.</param>
        /// <param name="abuseTypeDictionaryRepository">Abuse Type Dictionary repository.</param>
        public DictionaryProvider(
            ICollectionCacheProvider cacheProvider,
            ISizeTypeReadonlyRepository sizeTypeRepository,
            ISpaceAccessTypeReadonlyRepository spaceAccessTypeRepository,
            ISpaceTypeReadonlyRepository spaceTypeRepository,
            IPhoneVerificationStatusReadonlyRepository phoneVerificationStatusRepository,
            IEmailVerificationStatusReadonlyRepository emailVerificationStatusRepository,
            IMessageDeliveredStatusReadonlyRepository messageDeliveredStatusRepository,
            IMessageOfferStatusReadonlyRepository messageOfferStatusRepository,
            IRentPeriodTypeReadonlyRepository rentPeriodTypeRepository,
            IPaymentPointTypeReadonlyRepository paymentPointTypeRepository,
            IAbuseTypeDictionaryReadonlyRepository abuseTypeDictionaryRepository)
        {
            Contract.Requires(cacheProvider != null);
            Contract.Requires(sizeTypeRepository != null);
            Contract.Requires(spaceAccessTypeRepository != null);
            Contract.Requires(spaceTypeRepository != null);
            Contract.Requires(phoneVerificationStatusRepository != null);
			Contract.Requires(emailVerificationStatusRepository != null);
            Contract.Requires(messageDeliveredStatusRepository != null);
            Contract.Requires(messageOfferStatusRepository != null);
            Contract.Requires(rentPeriodTypeRepository != null);
            Contract.Requires(paymentPointTypeRepository != null);
            Contract.Requires(abuseTypeDictionaryRepository != null);

			this.cacheProvider = cacheProvider;
            this.emailVerificationStatusRepository = emailVerificationStatusRepository;
            this.sizeTypeRepository = sizeTypeRepository;
            this.spaceAccessTypeRepository = spaceAccessTypeRepository;
            this.spaceTypeRepository = spaceTypeRepository;
            this.phoneVerificationStatusRepository = phoneVerificationStatusRepository;
            this.messageDeliveredStatusRepository = messageDeliveredStatusRepository;
            this.messageOfferStatusRepository = messageOfferStatusRepository;
            this.rentPeriodTypeRepository = rentPeriodTypeRepository;
            this.paymentPointTypeRepository = paymentPointTypeRepository;
            this.abuseTypeDictionaryRepository = abuseTypeDictionaryRepository;
        }

        private IEnumerable<T> FetchCollection<T>(String key, Func<IQueryable<T>> dataFetchFunc)
        {
            IEnumerable<T> collection = cacheProvider.GetAll<T>(key);

            if (collection == null)
            {
                collection = dataFetchFunc().ToList();
                cacheProvider.CacheProvider.Set(key, collection);
            }

            return collection;
        }

        private readonly ICollectionCacheProvider cacheProvider;
        private readonly IEmailVerificationStatusReadonlyRepository emailVerificationStatusRepository;
        private readonly ISizeTypeReadonlyRepository sizeTypeRepository;
        private readonly ISpaceAccessTypeReadonlyRepository spaceAccessTypeRepository;
        private readonly ISpaceTypeReadonlyRepository spaceTypeRepository;
        private readonly IPhoneVerificationStatusReadonlyRepository phoneVerificationStatusRepository;
        private readonly IMessageDeliveredStatusReadonlyRepository messageDeliveredStatusRepository;
        private readonly IMessageOfferStatusReadonlyRepository messageOfferStatusRepository;
        private readonly IRentPeriodTypeReadonlyRepository rentPeriodTypeRepository;
        private readonly IPaymentPointTypeReadonlyRepository paymentPointTypeRepository;
        private readonly IAbuseTypeDictionaryReadonlyRepository abuseTypeDictionaryRepository;
    }
}
