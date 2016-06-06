namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Publisher exception.
    /// </summary>
    [Serializable]
    public class CommunicationException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CommunicationException() : base()
        { }

        /// <summary>
        /// Creates pusblisher exception with message.
        /// </summary>
        /// <param name="message">Message.</param>
        public CommunicationException(String message) : base(message)
        { }

        /// <summary>
        /// Creates publisher exception with message and inner exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public CommunicationException(String message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// Creates pusblisher exception from serialization context.
        /// </summary>
        /// <param name="info">Serialized info.</param>
        /// <param name="context">Context.</param>
        protected CommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
