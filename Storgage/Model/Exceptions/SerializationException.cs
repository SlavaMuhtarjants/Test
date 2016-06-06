namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Serialization exception.
    /// </summary>
    [Serializable]
    public class SerializationException : Exception
    {
        /// <summary>
        /// Creates serialization exception by default.
        /// </summary>
        public SerializationException() : base()
        {}

        /// <summary>
        /// Creates serialization exception with message.
        /// </summary>
        /// <param name="message">Message.</param>
        public SerializationException(String message) : base(message)
        { }

        /// <summary>
        /// Creates serialization exception with message and inner exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public SerializationException(String message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// Creates serialization exception using serialization context.
        /// </summary>
        /// <param name="info">Serialized info.</param>
        /// <param name="context">Serialization context.</param>
        protected SerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
