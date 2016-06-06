namespace Weezlabs.Storgage.UserNotifier.Serializators
{
    using System.Diagnostics.Contracts;
    using System.Collections.Generic;

    using Model.Enums;
    using UtilService;

    /// <summary>
    /// Message serializator factory.
    /// </summary>
    public class MessageSerializatorFactory : IMessageSerializatorFactory
    {
        private readonly IDictionary<MobileEndpointType, IMessageSerializator> serializators;


        /// <summary>
        /// Creates messagee serializator factory.
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        public MessageSerializatorFactory(IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            serializators = new Dictionary<MobileEndpointType, IMessageSerializator>()
            {
                { MobileEndpointType.Default, new DefaultJsonMessageSerializator() },
                { MobileEndpointType.Apns, new ApnsJsonMessageSerializator(appSettings) }
            };
        }

        /// <summary>
        /// Returns message serializator.
        /// </summary>
        /// <param name="endpointType">Mobile endpoint type.</param>
        /// <returns>Message serializator.</returns>
        public IMessageSerializator GetMessageSerializator(MobileEndpointType endpointType)
        {
            if (!serializators.ContainsKey(endpointType))
            {
                return serializators[MobileEndpointType.Default];
            }

            return serializators[endpointType];
        }
    }
}
