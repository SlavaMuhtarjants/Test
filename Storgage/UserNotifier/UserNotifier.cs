namespace Weezlabs.Storgage.UserNotifier
{
    using System;
    using System.Collections.Generic;    
    using System.Diagnostics.Contracts;
    using System.Linq;

    using MessagePublisher;
    using Model.Enums;
    using Notifications;
    using Serializators;
    using UserService;
    

    /// <summary>
    /// User notifier.
    /// </summary>
    public class UserNotifier : IUserNotifier
    {
        private readonly IUserDeviceProvider userDeviceProvider;
        private readonly IMobileEndpointProvider mobileEndpointProvider;
        private readonly IMessagePublisher messagePublisher;
        private readonly IMessageSerializatorFactory messageSerializatorFactory;

        /// <summary>
        /// Creates user notifier.
        /// </summary>
        /// <param name="userDeviceProvider">User device provider.</param>
        /// <param name="messagePublisher">Message publisher.</param>
        /// <param name="mobileEndpointProvider">Mobile endpoint provider.</param>
        /// <param name="messageSerializatorFactory">Message serializator factory.</param>
        public UserNotifier(IUserDeviceProvider userDeviceProvider,
            IMessagePublisher messagePublisher,
            IMobileEndpointProvider mobileEndpointProvider,
            IMessageSerializatorFactory messageSerializatorFactory)
        {
            Contract.Requires(userDeviceProvider != null);
            Contract.Requires(messagePublisher != null);
            Contract.Requires(mobileEndpointProvider != null);
            Contract.Requires(messageSerializatorFactory != null);

            this.messagePublisher = messagePublisher;
            this.messageSerializatorFactory = messageSerializatorFactory;
            this.userDeviceProvider = userDeviceProvider;
            this.mobileEndpointProvider = mobileEndpointProvider;
        }

        /// <summary>
        /// Sends message to recepient.
        /// </summary>      
        /// <param name="recepientId">Recepient identifier.</param>
        /// <param name="message">Message.</param>
        public void SendMessage(Guid recepientId, UserNotification message)
        {
            var devices = userDeviceProvider.GetUserDevices(recepientId)
                .Where(x => x.IsPushNotificationEnabled);
            var invalidatedDeviceIds = new List<Guid>();
            foreach (var device in devices)
            {
                var isEnabled = mobileEndpointProvider.IsMobileEndpointEnabled(device.MobileEndpoint);
                if (!isEnabled)
                {
                    invalidatedDeviceIds.Add(device.Id);
                    continue;
                }
                SendMessage(message, device.TopicEndpoint);
            }

            userDeviceProvider.InvalidateUserDevices(invalidatedDeviceIds);
        }

        private void SendMessage(UserNotification message, String topicEndpoint)
        {
            var messageSerializator = messageSerializatorFactory.GetMessageSerializator(MobileEndpointType.Apns);
            var serializedMessage = messageSerializator.SerializeMessage(message);
            messagePublisher.SendMessage(serializedMessage, topicEndpoint);
        }        
    }
}
