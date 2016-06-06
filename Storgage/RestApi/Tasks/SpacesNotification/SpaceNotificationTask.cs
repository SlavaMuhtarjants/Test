namespace Weezlabs.Storgage.RestApi.Tasks.SpacesNotification
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using DataLayer.Filters;
    using Model.Exceptions;
    using SpaceService;
    using UserNotifier;
    using UserNotifier.Notifications;
    using UtilService;

    using Castle.Core.Logging;
    using Quartz;


    /// <summary>
    /// Implements logic of notification delivery to users 
    /// about spaces matching their filters
    /// </summary>
    public class SpaceNotificationTask : ISpaceNotificationTask
    {
        private readonly IAppSettings appSettings;
        private readonly IFilterReadonlyRepository filterRepository;
        private readonly ISpaceSearcher spaceSearcher;
        private readonly IUserNotifier userNotifier;

        private ILogger logger = NullLogger.Instance;

        /// <summary>
        /// Logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        /// <summary>
        /// Creates instance of SpaceNotificationTask
        /// </summary>
        /// <param name="appSettings">app settings</param>
        /// <param name="userNotifier">user notifier</param>
        /// <param name="filterRepository">filter repository</param>
        /// <param name="spaceSearcher">space searching service</param>
        public SpaceNotificationTask(IAppSettings appSettings, IFilterReadonlyRepository filterRepository, 
            ISpaceSearcher spaceSearcher, IUserNotifier userNotifier)
        {
            Contract.Requires(appSettings != null);
            Contract.Requires(filterRepository != null);
            Contract.Requires(spaceSearcher != null);
            Contract.Requires(userNotifier != null);

            this.appSettings = appSettings;
            this.filterRepository = filterRepository;
            this.spaceSearcher = spaceSearcher;
            this.userNotifier = userNotifier;
        }

        /// <summary>
        /// Processes notifications delivery
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            Int32 pageSize = appSettings.GetSetting<Int32>("UserFilterNotificationBatchSize");
            Int32 offset = 0;
            Int32 totalUserCount = filterRepository.GetAll().Select(f => f.UserId).Distinct().Count();
            Int32 notificationCount = 0;

            Logger.Debug("The task of checking relevant spaces matching user filters has begun.");

            while (offset < totalUserCount)
            {
                Dictionary<Guid, Int32> report = null;
                Logger.DebugFormat("Selecting next {0} users.", pageSize);

                try
                {
                    report = spaceSearcher.CalculateSpacesByUsers(offset, pageSize);
                }
                catch (DataException ex)
                {
                    Exception exception = ex.InnerException ?? ex;
                    Logger.Error(exception.Message);

                    continue;
                }
                finally
                {
                    offset += pageSize;
                }

                Logger.DebugFormat("{0} users have new spaces satisfying their filter conditions.", report.Count());

                foreach (Guid userId in report.Keys)
                {
                    try
                    {
                        userNotifier.SendMessage(userId, new UserNotification
                        {
                            EventType = EventType.NewRelevantSpaces,
                            ObjectId = userId,
                            Message = String.Format(Resources.Messages.RelevantSpacesNotification, report[userId]),
                            Badge = report[userId]
                        });
                        notificationCount++;
                    }
                    catch (CommunicationException ex)
                    {
                        Logger.ErrorFormat("A exception has occured during sending push notification for the user {0}.{1}{2}",
                            userId, Environment.NewLine, ex.Message);
                    }
                }
            }

            Logger.Debug("The task of checking relevant spaces matching user filters has finished.");
            Logger.DebugFormat("Push notifications have been sent out to {0} users.", notificationCount);
        }
    }
}