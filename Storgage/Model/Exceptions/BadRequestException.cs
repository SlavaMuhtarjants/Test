namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Custom exception: bad request exception.
    /// </summary>
    [Serializable]
    public class BadRequestException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BadRequestException()
            : base()
        {
        }

        /// <summary>
        /// Creates BadRequestException.
        /// </summary>
        /// <param name="message">Error message.</param>
        public BadRequestException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates BadRequestException.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public BadRequestException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates BadRequestException for serialization.
        /// </summary>
        /// <param name="info">Serialization data.</param>
        /// <param name="context">Serialization context.</param>
        protected BadRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
