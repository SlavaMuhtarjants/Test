namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception for changing status exception.
    /// </summary>
    [Serializable]
    public class InvalidOfferStatusException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public InvalidOfferStatusException() : base()
        { }

        /// <summary>
        /// Creates custom exception with message.
        /// </summary>
        /// <param name="message">Message.</param>
        public InvalidOfferStatusException(String message) : base(message)
        { }

        /// <summary>
        /// Creates custom exception with message and inner exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public InvalidOfferStatusException(String message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// Creates custom exception from deserialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Serialization context.</param>
        protected InvalidOfferStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
