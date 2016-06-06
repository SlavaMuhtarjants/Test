namespace Weezlabs.Storgage.UserNotifier.Serializators
{
    using System;

    using Model.Exceptions;
    using Notifications;

    using Newtonsoft.Json;    

    /// <summary>
    /// Default json serializator.
    /// </summary>
    public class DefaultJsonMessageSerializator : IMessageSerializator
    {
        /// <summary>
        /// Serializes message to json.
        /// </summary>        
        /// <param name="message">Message.</param>
        /// <returns>Serialized to string message.</returns>
        public String SerializeMessage(UserNotification message)
        {
            return DoSerializeMessage(message);
        }

        /// <summary>
        /// Serializes message to json. Method for overriding.
        /// </summary>        
        /// <param name="message">Message.</param>
        /// <returns>Serialized to string message.</returns>
        protected virtual String DoSerializeMessage(UserNotification message)
        {
            try
            {
                var result = JsonConvert.SerializeObject(message);
                return result;
            }
            catch (JsonSerializationException ex)
            {
                throw new SerializationException(Resources.Messages.SerializerMessageException, ex);
            }    
        }
    }
}
