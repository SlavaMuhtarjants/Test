namespace Weezlabs.Storgage.MessagePublisher
{
    using System;
    /// <summary>
    /// Message publisher interface.
    /// </summary>    
    public interface IMessagePublisher
    {
        /// <summary>
        /// Sends message to publisher.
        /// </summary>
        /// <typeparam name="T">Message type.</typeparam>
        /// <param name="message">Message.</param>
        /// <param name="publisherEndpoint">Publisher endpoint.</param>
        /// <param name="messageSerializator">Message serializator.</param>
        void SendMessage(String message, String publisherEndpoint);
    }
}
