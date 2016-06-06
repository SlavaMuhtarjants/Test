namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Custom exception: not found exception.
    /// </summary>
    [Serializable]
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Default cconstructor.
        /// </summary>
        public NotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Creates NotFoundException.
        /// </summary>
        /// <param name="message">Error message.</param>
        public NotFoundException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates NotFoundException.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public NotFoundException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates NotFoundException for serialization.
        /// </summary>
        /// <param name="info">Serialization data.</param>
        /// <param name="context">Serialization context.</param>
        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
