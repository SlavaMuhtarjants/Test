namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;

    /// <summary>
    /// Contains information about when space is busy.
    /// </summary>
    public class SpaceBusySchedule
    {
        /// <summary>
        /// Start date when space is busy.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date when space is busy.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}