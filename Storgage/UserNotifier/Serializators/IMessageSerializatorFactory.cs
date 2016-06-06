namespace Weezlabs.Storgage.UserNotifier.Serializators
{
    using Model.Enums;

    /// <summary>
    /// Message serializator factory interface.
    /// </summary>
    public interface IMessageSerializatorFactory
    {
        /// <summary>
        /// Returns message serializator for mobile endpoint type.
        /// </summary>
        /// <param name="endpointType">Mobile endpoint type.</param>
        /// <returns>Message serializator.</returns>
        IMessageSerializator GetMessageSerializator(MobileEndpointType endpointType);
    }
}
