namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Delete files exception.
    /// </summary>
    [Serializable]
    public class DeleteFileException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DeleteFileException() : base()
        { }

        /// <summary>
        /// Creates delete files exception.
        /// </summary>
        /// <param name="message">Message.</param>
        public DeleteFileException(String message) : base(message)
        { }

        /// <summary>
        /// Creates delete files exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exeption.</param>
        public DeleteFileException(String message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// Creates delete files exception.
        /// </summary>
        /// <param name="info">Info to serialize.</param>
        /// <param name="context">Serializable context.</param>
        protected DeleteFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
