namespace Weezlabs.Storgage.UserNotifier
{
    using System;

    using Notifications;

    /// <summary>
    /// User notifier interface.
    /// </summary>
    public interface IUserNotifier
    {
        /// <summary>
        /// Sends message to recepient.
        /// </summary>     
        /// <param name="recepientId">Recepient identifier.</param>
        /// <param name="message">Message.</param>
        void SendMessage(Guid recepientId, UserNotification message);
    }
}
