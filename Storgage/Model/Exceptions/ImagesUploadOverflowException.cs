namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Upload images count overflow exception.
    /// </summary>
    [Serializable]
    public class ImagesUploadOverflowException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ImagesUploadOverflowException() : base()
        { }

        /// <summary>
        /// Creates upload images count overflow exception.
        /// </summary>
        /// <param name="message">Message.</param>
        public ImagesUploadOverflowException(String message) : base(message)
        { }

        /// <summary>
        /// Creates upload images count overflow exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exeption.</param>
        public ImagesUploadOverflowException(String message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// Creates upload images count overflow exception.
        /// </summary>
        /// <param name="info">Info to serialize.</param>
        /// <param name="context">Serializable context.</param>
        protected ImagesUploadOverflowException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
