namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Access denied exception.
    /// </summary>
    [Serializable]
    public class AccessDeniedException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AccessDeniedException() : base()
        { }

        /// <summary>
        /// Creates access denied exception.
        /// </summary>
        /// <param name="message">Message.</param>
        public AccessDeniedException(String message) : base(message)
        { }

        /// <summary>
        /// Creates access denied exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exeption.</param>
        public AccessDeniedException(String message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// Creates access denied exception.
        /// </summary>
        /// <param name="info">Info to serialize.</param>
        /// <param name="context">Serializable context.</param>
        protected AccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
