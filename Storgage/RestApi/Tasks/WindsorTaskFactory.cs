namespace Weezlabs.Storgage.RestApi.Tasks
{
    using System.Diagnostics.Contracts;

    using Castle.Windsor;
    using Quartz.Spi;    
    using Quartz;

    /// <summary>
    /// Task factory, that used Windsor castle IoC container to resolve task.
    /// </summary>
    public class WindsorTaskFactory : IJobFactory
    {
        private readonly IWindsorContainer container;

        /// <summary>
        /// Creates Windsor Task Factory.
        /// </summary>
        /// <param name="container">Windsor container.</param>
        public WindsorTaskFactory(IWindsorContainer container)
        {
            Contract.Requires(container != null);
            this.container = container;
        }

        /// <summary>
        /// Returns resolved task from IoC.
        /// </summary>
        /// <param name="bundle">Bundle.</param>
        /// <param name="scheduler">Scheduler.</param>
        /// <returns>Resolved task.</returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob) container.Resolve(bundle.JobDetail.JobType);
        }

        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="job">Task.</param>
        public void ReturnJob(IJob job)
        {
        }
    }
}
