namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception for not unique data.
    /// </summary>
    public class NotUniqueException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotUniqueException() : base()
        { }

        /// <summary>
        /// Creates custom exception with message..
        /// </summary>
        /// <param name="message">Message.</param>
        public NotUniqueException(String message) : base(message)
        { }

        /// <summary>
        /// Creates custom exception with message and inner exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public NotUniqueException(String message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// Creates custom exception from serialization context.
        /// </summary>
        /// <param name="info">Serialization data.</param>
        /// <param name="context">Serialization context.</param>
        protected NotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
