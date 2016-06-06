namespace Weezlabs.Storgage.RestApi.Tasks.OfferExpiration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using DataLayer;
    using DataLayer.ChatAndMessages;
    using DataLayer.Dictionaries;    
    using UserNotifier;
    using UtilService;

    using Castle.Core.Logging;
    using UserNotifier.Notifications;
    using Quartz;


    /// <summary>
    /// Offer expiration task.
    /// </summary>
    public class OfferExpirationTask : IOfferExpirationTask
    {
        private readonly IOfferRepository offerRepository;
        private readonly IOfferHistoryRepository offerHistoryRepository;
        private readonly IDictionaryProvider dictionaryProvider;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAppSettings appSettings;
        private readonly IUserNotifier userNotifier;

        /// <summary>
        /// Logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
        private ILogger logger = NullLogger.Instance;

        /// <summary>
        /// Creates offer expiration processor.
        /// </summary>
        /// <param name="offerRepository">Offer repository.</param>
        /// <param name="offerHistoryRepository">Offer history repository.</param>
        /// <param name="dictionaryProvider">Dictionary provider.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="userNotifier">User notifier.</param>
        public OfferExpirationTask(IOfferRepository offerRepository,
            IOfferHistoryRepository offerHistoryRepository,
            IDictionaryProvider dictionaryProvider,
            IUnitOfWork unitOfWork,
            IAppSettings appSettings,
            IUserNotifier userNotifier)
        {
            Contract.Requires(offerRepository != null);
            Contract.Requires(offerHistoryRepository != null);
            Contract.Requires(dictionaryProvider != null);
            Contract.Requires(unitOfWork != null);
            Contract.Requires(appSettings != null);
            Contract.Requires(userNotifier != null);

            this.offerRepository = offerRepository;
            this.offerHistoryRepository = offerHistoryRepository;
            this.dictionaryProvider = dictionaryProvider;
            this.unitOfWork = unitOfWork;
            this.appSettings = appSettings;
            this.userNotifier = userNotifier;
        }


        /// <summary>
        /// Executes offer expiration tasks.
        /// </summary>       
        public void Execute(IJobExecutionContext context)
        {
            Int32 errorsInARowCount = 0;
            Boolean continueProcessing = true;
            Int32 totalCount = 0;

            Logger.Debug("Started offer expiration processing.");

            while (continueProcessing)
            {
                try
                {
                    var batchSize = appSettings.GetSetting<Int32>("OfferExpirationBatchSize");
                    IEnumerable<ExpiredOfferInfo> offersInfo = ExpireOffers(batchSize, ref totalCount);

                    if (offersInfo.Count() == 0)
                    {
                        continueProcessing = false;
                    }
                    else
                    {
                        offersInfo.Distinct(new OfferComparer()).ToList().ForEach(x =>
                            userNotifier.SendMessage(x.UserID, 
                                new UserNotification
                                {
                                    EventType = EventType.OfferWasExpired,
                                    ObjectId = x.ChatID,
                                    Message = Resources.Messages.OfferWasExpiredNotification
                                }));                      
                    }

                    errorsInARowCount = 0;
                }
                catch (DataException ex)
                {
                    errorsInARowCount++;
                    Logger.Fatal("Database error.", GetOriginalException(ex));
                    if (errorsInARowCount == 5)
                    {
                        continueProcessing = false;
                    }
                }
            }

            Logger.Debug("Finished offer expiration processing.");
        }

        private IEnumerable<ExpiredOfferInfo> ExpireOffers(Int32 batchSize, ref Int32 totalCount)
        { 
            IEnumerable<ExpiredOfferInfo> offersInfo = OfferExpirationDataProvider.ExpireOffers(batchSize, offerRepository, offerHistoryRepository, dictionaryProvider, unitOfWork);
            Logger.Debug(String.Format("{0} offers have been updated.", offersInfo.Count()));
            totalCount += offersInfo.Count();

            return offersInfo;
        }
        private Exception GetOriginalException(Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            return exception;
        }
    }
}

