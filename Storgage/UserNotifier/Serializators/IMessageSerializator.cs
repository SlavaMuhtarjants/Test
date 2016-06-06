namespace Weezlabs.Storgage.UserNotifier.Serializators
{
    using System;

    using Notifications;

    /// <summary>
    /// Message serialization interface.
    /// </summary>
    public interface IMessageSerializator
    {
        /// <summary>
        /// Serializes message to string.
        /// </summary>        
        /// <param name="message">Message.</param>
        /// <returns>Serialized to string message.</returns>
        String SerializeMessage(UserNotification message);        
    }
}
