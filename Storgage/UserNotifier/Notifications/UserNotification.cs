namespace Weezlabs.Storgage.UserNotifier.Notifications
{
    using System;

    /// <summary>
    /// User notification container.
    /// </summary>
    public class UserNotification
    {
        /// <summary>
        /// Badge.
        /// </summary>
        public Int32? Badge { get; set; }

        /// <summary>
        /// Notification type.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Object identifier.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        public String Message { get; set; }
    }
}
