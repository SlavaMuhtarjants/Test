namespace Weezlabs.Storgage.RestApi.Tasks
{
    using System;

    using IoC;
    using UtilService;

    using Quartz;
    using Quartz.Impl;

    /// <summary>
    /// Task scheduler.
    /// </summary>
    public static class TaskScheduler
    {
        private static readonly IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

        /// <summary>
        /// Creates task scheuler.
        /// </summary>
        static TaskScheduler()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = new WindsorTaskFactory(ContainerWrapper.Container); 
        }

        /// <summary>
        /// Starts all registered tasks.
        /// </summary>
        public static void Start()
        {
            scheduler.Start();

            IJobDetail offerExpirationJob = JobBuilder.Create<OfferExpiration.IOfferExpirationTask>().Build();
            ITrigger offerExpirationTrigger = TriggerBuilder.Create().StartNow().WithSimpleSchedule
                (x => x.WithIntervalInHours(1).RepeatForever()).Build();
            scheduler.ScheduleJob(offerExpirationJob, offerExpirationTrigger);

            IJobDetail spaceNotificationJob = JobBuilder.Create<SpacesNotification.ISpaceNotificationTask>().Build();
            ITrigger spaceNotificationTrigger = TriggerBuilder.Create().StartNow().WithSchedule
                (CronScheduleBuilder.DailyAtHourAndMinute(GetFilterNotificationStartHour(), 0).InTimeZone(TimeZoneInfo.Utc)).Build();
            scheduler.ScheduleJob(spaceNotificationJob, spaceNotificationTrigger);
        }

        private static Int32 GetFilterNotificationStartHour()
        {
            Int32 startUTCHour = GetAppSetting().GetSetting<Int32>("UserFilterNotificationUTCStartHour");

            if (startUTCHour < 0 || startUTCHour > 23)
            {
                // run the notification sending task daily at 10 A.M. PST (Los Angeles time zone)
                startUTCHour = 17;
            }

            return startUTCHour;
        }

        private static IAppSettings GetAppSetting()
        {
            return ContainerWrapper.Container.Resolve<IAppSettings>();
        }

        /// <summary>
        /// Stops all registered tasks.
        /// </summary>
        public static void Stop()
        {
            scheduler.Shutdown();   
        }
    }
}
